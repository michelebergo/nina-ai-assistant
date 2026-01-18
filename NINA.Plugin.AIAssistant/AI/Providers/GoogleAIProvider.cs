using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NINA.Core.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Provider for Google AI (Gemini models)
    /// </summary>
    public class GoogleAIProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;

        public AIProviderType ProviderType => AIProviderType.GoogleAI;
        public string DisplayName => "Google AI (Gemini)";
        public bool IsConfigured => _httpClient != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri(config.Endpoint ?? "https://generativelanguage.googleapis.com/")
                };

                Logger.Info("Google AI provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Google AI provider: {ex.Message}");
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
                var model = _config.ModelId ?? "gemini-pro";
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = $"You are an expert astrophotography assistant for N.I.N.A. imaging software.\n\n{request.Prompt}" }
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

                var url = $"v1beta/models/{model}:generateContent?key={_config.ApiKey}";
                var response = await _httpClient.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObj = JObject.Parse(responseJson);

                return new AIResponse
                {
                    Success = true,
                    Content = responseObj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString(),
                    ModelUsed = model,
                    Metadata = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["provider"] = "Google AI",
                        ["finish_reason"] = responseObj["candidates"]?[0]?["finishReason"]?.ToString() ?? "unknown"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Google AI request failed: {ex.Message}");
                return new AIResponse
                {
                    Success = false,
                    Error = ex.Message
                };
            }
        }

        public async Task<bool> TestConnectionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var testRequest = new AIRequest
                {
                    Prompt = "Test",
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
            return await Task.FromResult(new[]
            {
                "gemini-2.0-flash",
                "gemini-1.5-pro",
                "gemini-1.5-flash",
                "gemini-pro"
            });
        }
    }
}
