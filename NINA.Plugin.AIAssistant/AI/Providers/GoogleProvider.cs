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
    /// Provider for Google Gemini API (free tier available)
    /// </summary>
    public class GoogleProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;
        private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models";

        public AIProviderType ProviderType => AIProviderType.Google;
        public string DisplayName => "Google Gemini (Free tier)";
        public bool IsConfigured => _httpClient != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;

                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Logger.Info("Google Gemini provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Google Gemini provider: {ex.Message}");
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
                // Latest: gemini-2.0-flash-exp (fastest, multimodal), gemini-1.5-pro (most capable)
                var modelId = _config.ModelId ?? "gemini-2.0-flash-exp";
                var systemInstruction = request.SystemPrompt ?? "You are an expert astrophotography assistant for N.I.N.A. (Nighttime Imaging 'N' Astronomy). Help analyze images, suggest optimal settings, and provide intelligent guidance.";

                var requestBody = new
                {
                    system_instruction = new
                    {
                        parts = new[]
                        {
                            new { text = systemInstruction }
                        }
                    },
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = request.Prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = request.Temperature,
                        maxOutputTokens = request.MaxTokens
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{BaseUrl}/{modelId}:generateContent?key={_config.ApiKey}";
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    Logger.Error($"Google Gemini API error: {responseContent}");
                    return new AIResponse { Success = false, Error = $"API Error: {response.StatusCode} - {responseContent}" };
                }

                var jsonResponse = JObject.Parse(responseContent);
                var messageContent = jsonResponse["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                var tokensUsed = jsonResponse["usageMetadata"]?["totalTokenCount"]?.Value<int>();

                return new AIResponse
                {
                    Success = true,
                    Content = messageContent,
                    ModelUsed = modelId,
                    TokensUsed = tokensUsed,
                    Metadata = new Dictionary<string, object>
                    {
                        ["provider"] = "Google"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Google Gemini request failed: {ex.Message}");
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
                    MaxTokens = 20
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
            return await Task.FromResult(new[]
            {
                "gemini-1.5-flash",
                "gemini-1.5-pro",
                "gemini-2.0-flash-exp"
            });
        }
    }
}
