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
    /// Provider for Anthropic Claude models
    /// </summary>
    public class AnthropicProvider : IAIProvider
    {
        private HttpClient? _httpClient;
        private AIProviderConfig? _config;

        public AIProviderType ProviderType => AIProviderType.Anthropic;
        public string DisplayName => "Anthropic Claude";
        public bool IsConfigured => _httpClient != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri(config.Endpoint ?? "https://api.anthropic.com/")
                };
                _httpClient.DefaultRequestHeaders.Add("x-api-key", config.ApiKey);
                _httpClient.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                Logger.Info("Anthropic provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Anthropic provider: {ex.Message}");
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
                    model = _config.ModelId ?? "claude-sonnet-4-5",
                    max_tokens = request.MaxTokens,
                    temperature = request.Temperature,
                    system = "You are an expert astrophotography assistant for N.I.N.A. imaging software.",
                    messages = new[]
                    {
                        new { role = "user", content = request.Prompt }
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("v1/messages", content, cancellationToken);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObj = JObject.Parse(responseJson);

                return new AIResponse
                {
                    Success = true,
                    Content = responseObj["content"]?[0]?["text"]?.ToString(),
                    ModelUsed = responseObj["model"]?.ToString(),
                    TokensUsed = responseObj["usage"]?["input_tokens"]?.Value<int>() + responseObj["usage"]?["output_tokens"]?.Value<int>(),
                    Metadata = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["provider"] = "Anthropic",
                        ["stop_reason"] = responseObj["stop_reason"]?.ToString() ?? "unknown"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Anthropic request failed: {ex.Message}");
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
                "claude-opus-4-5",
                "claude-sonnet-4-5",
                "claude-haiku-4-5",
                "claude-3-opus",
                "claude-3-sonnet",
                "claude-3-haiku"
            });
        }
    }
}
