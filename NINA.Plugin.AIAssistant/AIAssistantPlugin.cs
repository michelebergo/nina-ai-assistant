using NINA.Plugin.AIAssistant.Properties;
using NINA.Core.Utility;
using NINA.Plugin;
using NINA.Plugin.Interfaces;
using NINA.Profile.Interfaces;
using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using NINA.Plugin.AIAssistant.AI;

namespace NINA.Plugin.AIAssistant
{
    [Export(typeof(IPluginManifest))]
    public class AIAssistantPlugin : PluginBase, INotifyPropertyChanged
    {
        private readonly IPluginOptionsAccessor pluginSettings;
        private readonly IProfileService profileService;
        private readonly AIService aiService;

        [ImportingConstructor]
        public AIAssistantPlugin(IProfileService profileService)
        {
            if (Settings.Default.UpdateSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpdateSettings = false;
                CoreUtil.SaveSettings(Settings.Default);
            }

            this.pluginSettings = new PluginOptionsAccessor(profileService, Guid.Parse(this.Identifier));
            this.profileService = profileService;
            this.aiService = new AIService();

            // Initialize AI providers based on saved settings
            _ = InitializeAIProvidersAsync();

            Logger.Info("NINA AI Assistant Plugin loaded successfully");
        }

        private async Task InitializeAIProvidersAsync()
        {
            try
            {
                // Initialize GitHub Models provider if configured
                if (GitHubEnabled && !string.IsNullOrEmpty(GitHubApiKey))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.GitHub,
                        ApiKey = GitHubApiKey,
                        ModelId = GitHubModelId,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Initialize Microsoft Foundry provider if configured
                if (FoundryEnabled && !string.IsNullOrEmpty(FoundryApiKey) && !string.IsNullOrEmpty(FoundryEndpoint))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.MicrosoftFoundry,
                        ApiKey = FoundryApiKey,
                        Endpoint = FoundryEndpoint,
                        DeploymentName = FoundryDeploymentName,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Initialize Azure OpenAI provider if configured
                if (AzureOpenAIEnabled && !string.IsNullOrEmpty(AzureOpenAIApiKey) && !string.IsNullOrEmpty(AzureOpenAIEndpoint))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.AzureOpenAI,
                        ApiKey = AzureOpenAIApiKey,
                        Endpoint = AzureOpenAIEndpoint,
                        DeploymentName = AzureOpenAIDeploymentName,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Initialize OpenAI provider if configured
                if (OpenAIEnabled && !string.IsNullOrEmpty(OpenAIApiKey))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.OpenAI,
                        ApiKey = OpenAIApiKey,
                        ModelId = OpenAIModelId,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Initialize Anthropic provider if configured
                if (AnthropicEnabled && !string.IsNullOrEmpty(AnthropicApiKey))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.Anthropic,
                        ApiKey = AnthropicApiKey,
                        ModelId = AnthropicModelId,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Initialize Google AI provider if configured
                if (GoogleAIEnabled && !string.IsNullOrEmpty(GoogleAIApiKey))
                {
                    var config = new AIProviderConfig
                    {
                        ProviderType = AIProviderType.GoogleAI,
                        ApiKey = GoogleAIApiKey,
                        ModelId = GoogleAIModelId,
                        IsEnabled = true
                    };
                    await aiService.InitializeProviderAsync(config);
                }

                // Set active provider
                aiService.SetActiveProvider(ActiveProviderType);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error initializing AI providers: {ex.Message}");
            }
        }

        public AIService GetAIService() => aiService;

        public override Task Teardown()
        {
            return base.Teardown();
        }

        #region GitHub Models Settings

        public bool GitHubEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(GitHubEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(GitHubEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string GitHubApiKey
        {
            get => pluginSettings.GetValueString(nameof(GitHubApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(GitHubApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string GitHubModelId
        {
            get => pluginSettings.GetValueString(nameof(GitHubModelId), "gpt-4o-mini");
            set
            {
                pluginSettings.SetValueString(nameof(GitHubModelId), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Microsoft Foundry Settings

        public bool FoundryEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(FoundryEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(FoundryEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string FoundryApiKey
        {
            get => pluginSettings.GetValueString(nameof(FoundryApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(FoundryApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string FoundryEndpoint
        {
            get => pluginSettings.GetValueString(nameof(FoundryEndpoint), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(FoundryEndpoint), value);
                RaisePropertyChanged();
            }
        }

        public string FoundryDeploymentName
        {
            get => pluginSettings.GetValueString(nameof(FoundryDeploymentName), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(FoundryDeploymentName), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Azure OpenAI Settings

        public bool AzureOpenAIEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(AzureOpenAIEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(AzureOpenAIEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string AzureOpenAIApiKey
        {
            get => pluginSettings.GetValueString(nameof(AzureOpenAIApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(AzureOpenAIApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string AzureOpenAIEndpoint
        {
            get => pluginSettings.GetValueString(nameof(AzureOpenAIEndpoint), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(AzureOpenAIEndpoint), value);
                RaisePropertyChanged();
            }
        }

        public string AzureOpenAIDeploymentName
        {
            get => pluginSettings.GetValueString(nameof(AzureOpenAIDeploymentName), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(AzureOpenAIDeploymentName), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region OpenAI Settings

        public bool OpenAIEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(OpenAIEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(OpenAIEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string OpenAIApiKey
        {
            get => pluginSettings.GetValueString(nameof(OpenAIApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(OpenAIApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string OpenAIModelId
        {
            get => pluginSettings.GetValueString(nameof(OpenAIModelId), "gpt-4o-mini");
            set
            {
                pluginSettings.SetValueString(nameof(OpenAIModelId), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Anthropic Settings

        public bool AnthropicEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(AnthropicEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(AnthropicEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string AnthropicApiKey
        {
            get => pluginSettings.GetValueString(nameof(AnthropicApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(AnthropicApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string AnthropicModelId
        {
            get => pluginSettings.GetValueString(nameof(AnthropicModelId), "claude-sonnet-4-5");
            set
            {
                pluginSettings.SetValueString(nameof(AnthropicModelId), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Google AI Settings

        public bool GoogleAIEnabled
        {
            get => pluginSettings.GetValueBoolean(nameof(GoogleAIEnabled), false);
            set
            {
                pluginSettings.SetValueBoolean(nameof(GoogleAIEnabled), value);
                RaisePropertyChanged();
            }
        }

        public string GoogleAIApiKey
        {
            get => pluginSettings.GetValueString(nameof(GoogleAIApiKey), string.Empty);
            set
            {
                pluginSettings.SetValueString(nameof(GoogleAIApiKey), value);
                RaisePropertyChanged();
            }
        }

        public string GoogleAIModelId
        {
            get => pluginSettings.GetValueString(nameof(GoogleAIModelId), "gemini-pro");
            set
            {
                pluginSettings.SetValueString(nameof(GoogleAIModelId), value);
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Active Provider

        public AIProviderType ActiveProviderType
        {
            get => (AIProviderType)pluginSettings.GetValueInt32(nameof(ActiveProviderType), 0);
            set
            {
                pluginSettings.SetValueInt32(nameof(ActiveProviderType), (int)value);
                aiService.SetActiveProvider(value);
                RaisePropertyChanged();
            }
        }

        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
