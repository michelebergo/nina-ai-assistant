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
    /// Provider for OpenAI API (direct)
    /// </summary>
    public class OpenAIProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;

        public AIProviderType ProviderType => AIProviderType.OpenAI;
        public string DisplayName => "OpenAI";
        public bool IsConfigured => _httpClient != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri(config.Endpoint ?? "https://api.openai.com/v1/")
                };
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {config.ApiKey}");

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
                var requestBody = new
                {
                    model = _config.ModelId ?? "gpt-4o-mini",
                    messages = new[]
                    {
                        new { role = "system", content = "You are an expert astrophotography assistant for N.I.N.A. imaging software." },
                        new { role = "user", content = request.Prompt }
                    },
                    temperature = request.Temperature,
                    max_tokens = request.MaxTokens
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("chat/completions", content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObj = JObject.Parse(responseJson);

                return new AIResponse
                {
                    Success = true,
                    Content = responseObj["choices"]?[0]?["message"]?["content"]?.ToString(),
                    ModelUsed = responseObj["model"]?.ToString(),
                    TokensUsed = responseObj["usage"]?["total_tokens"]?.Value<int>(),
                    Metadata = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["provider"] = "OpenAI",
                        ["finish_reason"] = responseObj["choices"]?[0]?["finish_reason"]?.ToString() ?? "unknown"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"OpenAI request failed: {ex.Message}");
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
                    MaxTokens = 5
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
                "gpt-4o",
                "gpt-4o-mini",
                "gpt-4-turbo",
                "gpt-4",
                "gpt-3.5-turbo",
                "o1-preview",
                "o1-mini"
            });
        }
    }
}
