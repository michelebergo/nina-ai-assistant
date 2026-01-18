using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NINA.Core.Utility;

namespace NINA.Plugin.AIAssistant.AI
{
    /// <summary>
    /// Service that manages multiple AI providers and routes requests
    /// </summary>
    public class AIService
    {
        private readonly Dictionary<AIProviderType, IAIProvider> _providers = new();
        private AIProviderType _activeProvider = AIProviderType.GitHub;

        public AIService()
        {
            // Register all available providers
            RegisterProvider(new GitHubModelsProvider());
            RegisterProvider(new MicrosoftFoundryProvider());
            RegisterProvider(new AzureOpenAIProvider());
            RegisterProvider(new OpenAIProvider());
            RegisterProvider(new AnthropicProvider());
            RegisterProvider(new GoogleAIProvider());
        }

        /// <summary>
        /// Register an AI provider
        /// </summary>
        public void RegisterProvider(IAIProvider provider)
        {
            _providers[provider.ProviderType] = provider;
        }

        /// <summary>
        /// Get all registered providers
        /// </summary>
        public IEnumerable<IAIProvider> GetProviders() => _providers.Values;

        /// <summary>
        /// Get a specific provider by type
        /// </summary>
        public IAIProvider? GetProvider(AIProviderType type)
        {
            return _providers.TryGetValue(type, out var provider) ? provider : null;
        }

        /// <summary>
        /// Set the active provider
        /// </summary>
        public void SetActiveProvider(AIProviderType type)
        {
            if (_providers.ContainsKey(type))
            {
                _activeProvider = type;
                Logger.Info($"AI Service: Active provider set to {type}");
            }
        }

        /// <summary>
        /// Get the currently active provider
        /// </summary>
        public IAIProvider? GetActiveProvider()
        {
            return _providers.TryGetValue(_activeProvider, out var provider) ? provider : null;
        }

        /// <summary>
        /// Initialize a provider with configuration
        /// </summary>
        public async Task<bool> InitializeProviderAsync(AIProviderConfig config, CancellationToken cancellationToken = default)
        {
            var provider = GetProvider(config.ProviderType);
            if (provider == null)
            {
                Logger.Error($"AI Service: Provider {config.ProviderType} not found");
                return false;
            }

            try
            {
                var result = await provider.InitializeAsync(config, cancellationToken);
                if (result)
                {
                    Logger.Info($"AI Service: Successfully initialized {config.ProviderType}");
                }
                else
                {
                    Logger.Warning($"AI Service: Failed to initialize {config.ProviderType}");
                }
                return result;
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Service: Error initializing {config.ProviderType}: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Send a request using the active provider
        /// </summary>
        public async Task<AIResponse> SendRequestAsync(AIRequest request, CancellationToken cancellationToken = default)
        {
            var provider = GetActiveProvider();
            if (provider == null || !provider.IsConfigured)
            {
                return new AIResponse
                {
                    Success = false,
                    Error = "No AI provider configured. Please configure an AI provider in plugin settings."
                };
            }

            try
            {
                return await provider.SendRequestAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Service: Error sending request: {ex.Message}");
                return new AIResponse
                {
                    Success = false,
                    Error = $"AI request failed: {ex.Message}"
                };
            }
        }

        /// <summary>
        /// Test connection for all configured providers
        /// </summary>
        public async Task<Dictionary<AIProviderType, bool>> TestAllConnectionsAsync(CancellationToken cancellationToken = default)
        {
            var results = new Dictionary<AIProviderType, bool>();

            foreach (var provider in _providers.Values.Where(p => p.IsConfigured))
            {
                try
                {
                    var result = await provider.TestConnectionAsync(cancellationToken);
                    results[provider.ProviderType] = result;
                }
                catch (Exception ex)
                {
                    Logger.Error($"AI Service: Connection test failed for {provider.ProviderType}: {ex.Message}");
                    results[provider.ProviderType] = false;
                }
            }

            return results;
        }
    }
}
