using System;
using System.Collections.Generic;
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
    /// Provider for OpenRouter (aggregator with free and paid models)
    /// </summary>
    public class OpenRouterProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;
        private const string BaseUrl = "https://openrouter.ai/api/v1";

        public AIProviderType ProviderType => AIProviderType.OpenRouter;
        public string DisplayName => "OpenRouter";
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
                _httpClient.DefaultRequestHeaders.Add("HTTP-Referer", "https://github.com/nina-ai-assistant");
                _httpClient.DefaultRequestHeaders.Add("X-Title", "NINA AI Assistant");

                Logger.Info("OpenRouter provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize OpenRouter provider: {ex.Message}");
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

                var requestBody = new
                {
                    model = _config.ModelId ?? "meta-llama/llama-3.2-3b-instruct:free",
                    messages = messages,
                    temperature = request.Temperature,
                    max_tokens = request.MaxTokens
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{BaseUrl}/chat/completions", content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"OpenRouter API error: {responseContent}");
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
                        ["provider"] = "OpenRouter"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"OpenRouter request failed: {ex.Message}");
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
            // Popular free and paid models on OpenRouter
            return await Task.FromResult(new[]
            {
                // Free models
                "meta-llama/llama-3.2-3b-instruct:free",
                "google/gemma-2-9b-it:free",
                "mistralai/mistral-7b-instruct:free",
                "huggingfaceh4/zephyr-7b-beta:free",
                // Paid but cheap
                "openai/gpt-4o-mini",
                "anthropic/claude-3.5-haiku",
                // Premium
                "openai/gpt-4o",
                "anthropic/claude-3.5-sonnet",
                "google/gemini-pro-1.5"
            });
        }
    }
}
