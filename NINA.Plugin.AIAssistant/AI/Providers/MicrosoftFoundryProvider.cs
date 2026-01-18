using System;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.AI.Inference;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Provider for Microsoft Foundry (formerly Azure AI Foundry)
    /// Enterprise-grade deployment with advanced features
    /// </summary>
    public class MicrosoftFoundryProvider : IAIProvider
    {
        private ChatCompletionsClient? _client;
        private AIProviderConfig? _config;

        public AIProviderType ProviderType => AIProviderType.MicrosoftFoundry;
        public string DisplayName => "Microsoft Foundry";
        public bool IsConfigured => _client != null && _config != null;

        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                _config = config;

                if (string.IsNullOrEmpty(config.Endpoint))
                {
                    throw new ArgumentException("Endpoint is required for Microsoft Foundry");
                }

                var credential = new AzureKeyCredential(config.ApiKey ?? throw new ArgumentException("API key is required"));
                _client = new ChatCompletionsClient(new Uri(config.Endpoint), credential);

                Logger.Info("Microsoft Foundry provider initialized successfully");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to initialize Microsoft Foundry provider: {ex.Message}");
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
                    new ChatRequestSystemMessage("You are an expert astrophotography assistant for N.I.N.A. Analyze images, optimize settings, and provide intelligent guidance based on imaging conditions."),
                    new ChatRequestUserMessage(request.Prompt)
                };

                var chatOptions = new ChatCompletionsOptions
                {
                    Messages = { messages[0], messages[1] },
                    Temperature = (float)request.Temperature,
                    MaxTokens = request.MaxTokens,
                    Model = _config.DeploymentName ?? _config.ModelId ?? throw new ArgumentException("Model or deployment name is required")
                };

                var response = await _client.CompleteAsync(chatOptions, cancellationToken);

                return new AIResponse
                {
                    Success = true,
                    Content = response.Value.Choices[0].Message.Content,
                    ModelUsed = response.Value.Model,
                    TokensUsed = response.Value.Usage.TotalTokens,
                    Metadata = new System.Collections.Generic.Dictionary<string, object>
                    {
                        ["provider"] = "Microsoft Foundry",
                        ["finish_reason"] = response.Value.Choices[0].FinishReason.ToString()
                    }
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Microsoft Foundry request failed: {ex.Message}");
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
            // Common Foundry models
            return await Task.FromResult(new[]
            {
                "gpt-5.2",
                "gpt-5.1",
                "gpt-4.1",
                "claude-opus-4-5",
                "claude-sonnet-4-5",
                "o3",
                "o1"
            });
        }
    }
}
