using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Inference;
using NINA.Core.Utility;
using Newtonsoft.Json;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Provider for GitHub Models (free tier available)
    /// Uses the Azure AI Inference SDK with GitHub endpoint
    /// </summary>
    public class GitHubModelsProvider : IAIProvider
    {
        private ChatCompletionsClient? _client;
        private AIProviderConfig? _config;

        public AIProviderType ProviderType => AIProviderType.GitHub;
        public string DisplayName => "GitHub Models (Free)";
        public bool IsConfigured => _client != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;

                // GitHub Models endpoint
                var endpoint = new Uri("https://models.inference.ai.azure.com");
                
                // GitHub PAT or API key
                var credential = new AzureKeyCredential(config.ApiKey ?? throw new ArgumentException("API key is required"));

                _client = new ChatCompletionsClient(endpoint, credential);

                Logger.Info("GitHub Models provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize GitHub Models provider: {ex.Message}");
                return false;
            }
        }

        public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            if (_client == null || _config == null)
            {
                return new AIResponse { Success = false, Error = "Provider not initialized" };
            }

            try
            {
                var messages = new ChatRequestMessage[]
                {
                    new ChatRequestSystemMessage("You are an expert astrophotography assistant for N.I.N.A. (Nighttime Imaging 'N' Astronomy). Help analyze images, suggest optimal settings, and provide intelligent guidance."),
                    new ChatRequestUserMessage(request.Prompt)
                };

                var chatOptions = new ChatCompletionsOptions
                {
                    Messages = { messages[0], messages[1] },
                    Temperature = (float)request.Temperature,
                    MaxTokens = request.MaxTokens,
                    Model = _config.ModelId ?? "gpt-4o-mini" // Default to gpt-4o-mini for cost efficiency
                };

                var response = await _client.CompleteAsync(chatOptions, cancellationToken);
                var result = response.Value;
                var firstChoice = result.Content;

                return new AIResponse
                {
                    Success = true,
                    Content = firstChoice,
                    ModelUsed = result.Model ?? _config.ModelId,
                    Metadata = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["provider"] = "GitHub"
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"GitHub Models request failed: {ex.Message}");
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
            // GitHub Models available as of January 2026
            return await Task.FromResult(new[]
            {
                "gpt-4o",
                "gpt-4o-mini",
                "gpt-4.1",
                "gpt-4.1-mini",
                "gpt-5",
                "gpt-5-mini",
                "o1",
                "o1-mini",
                "o3-mini",
                "claude-sonnet-4-5",
                "llama-3.3-70b-instruct",
                "phi-4"
            });
        }
    }
}
