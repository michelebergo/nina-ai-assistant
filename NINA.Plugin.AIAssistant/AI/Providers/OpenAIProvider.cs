using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Provider for OpenAI API (GPT-4, GPT-4o, etc.)
    /// </summary>
    public class OpenAIProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;
        private const string BaseUrl = "https://api.openai.com/v1";

        public AIProviderType ProviderType => AIProviderType.OpenAI;
        public string DisplayName => "OpenAI";
        public bool IsConfigured => _httpClient != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;

                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new AuthenticationHeaderValue("Bearer", config.ApiKey ?? throw new ArgumentException("API key is required"));
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Logger.Info("OpenAI provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize OpenAI provider: {ex.Message}");
                return false;
            }
        }

        public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            if (_httpClient == null || _config == null)
            {
                return new AIResponse { Success = false, Error = "Provider not initialized" };
            }

            try
            {
                var messages = new List<object>
                {
                    new { 
                        role = "system", 
                        content = request.SystemPrompt ?? "You are an expert astrophotography assistant for N.I.N.A. (Nighttime Imaging 'N' Astronomy). Help analyze images, suggest optimal settings, and provide intelligent guidance."
                    },
                    new { role = "user", content = request.Prompt }
                };

                var modelId = _config.ModelId ?? "gpt-4o";
                
                // GPT-4o and newer models require max_completion_tokens instead of max_tokens
                var useMaxCompletionTokens = modelId.Contains("gpt-4o") || modelId.Contains("o1") || modelId.Contains("o3");
                
                var requestBody = new Dictionary<string, object>
                {
                    ["model"] = modelId,
                    ["messages"] = messages,
                    ["temperature"] = request.Temperature
                };
                
                if (useMaxCompletionTokens)
                {
                    requestBody["max_completion_tokens"] = request.MaxTokens;
                }
                else
                {
                    requestBody["max_tokens"] = request.MaxTokens;
                }

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/chat/completions", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"OpenAI API error: {responseContent}");
                    return new AIResponse { Success = false, Error = $"API Error: {response.StatusCode} - {responseContent}" };
                }

                var jsonResponse = JObject.Parse(responseContent);
                var messageContent = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString();
                var tokensUsed = jsonResponse["usage"]?["total_tokens"]?.Value<int>();
                var modelUsed = jsonResponse["model"]?.ToString();

                return new AIResponse
                {
                    Success = true,
                    Content = messageContent,
                    ModelUsed = modelUsed ?? _config.ModelId,
                    TokensUsed = tokensUsed,
                    Metadata = new Dictionary<string, object>
                    {
                        ["provider"] = "OpenAI"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"OpenAI request failed: {ex.Message}");
                return new AIResponse { Success = false, Error = ex.Message };
            }
        }

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var testRequest = new AIRequest
                {
                    Prompt = "Hello, confirm you're working.",
                    MaxTokens = 10
                };

                var response = await SendRequestAsync(testRequest, cancellationToken);
                return response.Success;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string[]> GetAvailableModelsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_httpClient == null || _config == null)
                    return GetDefaultModels();

                var request = new HttpRequestMessage(HttpMethod.Get, "https://api.openai.com/v1/models");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);

                var response = await _httpClient.SendAsync(request, cancellationToken);
                
                if (!response.IsSuccessStatusCode)
                    return GetDefaultModels();

                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
                var jsonResponse = JObject.Parse(responseContent);
                var models = jsonResponse["data"]?.ToObject<List<JObject>>();

                if (models == null || models.Count == 0)
                    return GetDefaultModels();

                // Filter to only chat/completion models (exclude embeddings, audio, etc.)
                var modelIds = models
                    .Select(m => m["id"]?.ToString())
                    .Where(id => !string.IsNullOrEmpty(id) && 
                                (id.StartsWith("gpt") || id.StartsWith("o1") || id.StartsWith("o3")))
                    .OrderByDescending(id => id) // Latest versions first
                    .ToArray();

                return modelIds.Length > 0 ? modelIds : GetDefaultModels();
            }
            catch (Exception ex)
            {
                Logger.Warning($"Failed to fetch OpenAI models: {ex.Message}");
                return GetDefaultModels();
            }
        }

        private string[] GetDefaultModels()
        {
            return new[]
            {
                "gpt-4o",
                "gpt-4o-mini",
                "gpt-4-turbo",
                "gpt-3.5-turbo",
                "o1-mini",
                "o1-preview"
            };
        }
    }
}
