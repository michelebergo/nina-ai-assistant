using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// AI service supporting multiple providers
    /// </summary>
    public class AIService
    {
        private IAIProvider? _activeProvider;
        private AIProviderType _activeProviderType;
        private readonly Dictionary<AIProviderType, IAIProvider> _providers;

        public AIService()
        {
            _providers = new Dictionary<AIProviderType, IAIProvider>
            {
                { AIProviderType.GitHub, new GitHubModelsProvider() },
                { AIProviderType.OpenAI, new OpenAIProvider() },
                { AIProviderType.Anthropic, new AnthropicProvider() },
                { AIProviderType.Google, new GoogleProvider() },
                { AIProviderType.Ollama, new OllamaProvider() },
                { AIProviderType.OpenRouter, new OpenRouterProvider() }
            };
        }

        /// <summary>
        /// Currently active provider type
        /// </summary>
        public AIProviderType ActiveProviderType => _activeProviderType;

        /// <summary>
        /// Initialize a specific provider
        /// </summary>
        public async Task<bool> InitializeAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_providers.TryGetValue(config.Provider, out var provider))
                {
                    Logger.Error($"AI Service: Unknown provider type {config.Provider}");
                    return false;
                }

                var result = await provider.InitializeAsync(config, cancellationToken);
                if (result)
                {
                    _activeProvider = provider;
                    _activeProviderType = config.Provider;
                    Logger.Info($"AI Service: {provider.DisplayName} initialized successfully");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Service: Failed to initialize provider - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Test connection for a specific provider
        /// </summary>
        public async Task<bool> TestConnectionAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!_providers.TryGetValue(config.Provider, out var provider))
                {
                    return false;
                }

                // Initialize temporarily for testing
                var initResult = await provider.InitializeAsync(config, cancellationToken);
                if (!initResult)
                    return false;

                return await provider.TestConnectionAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Service: Connection test failed - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send a request to the active provider
        /// </summary>
        public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            if (_activeProvider == null || !_activeProvider.IsConfigured)
            {
                return new AIResponse
                {
                    Success = false,
                    Error = "No AI provider configured. Please configure a provider in plugin settings."
                };
            }

            try
            {
                Logger.Info($"AI Service: Sending request to {_activeProvider.DisplayName}");
                var response = await _activeProvider.SendRequestAsync(request, cancellationToken);
                
                if (response.Success)
                {
                    Logger.Info($"AI Service: Request completed successfully using {_activeProvider.DisplayName}");
                }
                else
                {
                    Logger.Warning($"AI Service: Request failed - {response.Error}");
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Service: Request failed with exception - {ex.Message}");
                return new AIResponse
                {
                    Success = false,
                    Error = $"Request failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Simple query method for chat interactions
        /// </summary>
        public async Task<string> QueryAsync(string userMessage, string? systemPrompt = null, CancellationToken cancellationToken = default)
        {
            var request = new AIRequest
            {
                Prompt = userMessage,
                SystemPrompt = systemPrompt,
                MaxTokens = 1024,
                Temperature = 0.7
            };

            var response = await SendRequestAsync(request, cancellationToken);
            
            if (!response.Success)
            {
                throw new Exception(response.Error ?? "Unknown error");
            }

            return response.Content ?? "No response received";
        }

        /// <summary>
        /// Get available models for a provider
        /// </summary>
        public async Task<string[]> GetAvailableModelsAsync(AIProviderType providerType, CancellationToken cancellationToken = default)
        {
            if (_providers.TryGetValue(providerType, out var provider))
            {
                return await provider.GetAvailableModelsAsync(cancellationToken);
            }
            return Array.Empty<string>();
        }

        /// <summary>
        /// Check if the service is configured and ready
        /// </summary>
        public bool IsReady => _activeProvider?.IsConfigured ?? false;

        /// <summary>
        /// Get the display name of the active provider
        /// </summary>
        public string? ActiveProviderName => _activeProvider?.DisplayName;

        /// <summary>
        /// Get the active provider instance (for MCP initialization)
        /// </summary>
        public IAIProvider? GetActiveProvider() => _activeProvider;
    }
}
