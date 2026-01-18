using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NINA.Core.Utility;
using NINA.Plugin.AIAssistant.AI;
using NINA.Plugin.AIAssistant.AI.MCP;
using NINA.Equipment.Interfaces.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

// Resolve ambiguity - use NINA's RelayCommand for simple commands
using MvvmRelayCommand = CommunityToolkit.Mvvm.Input.RelayCommand;
using MvvmAsyncRelayCommand = CommunityToolkit.Mvvm.Input.AsyncRelayCommand;

namespace NINA.Plugin.AIAssistant
{
    [Export(typeof(IDockableVM))]
    public class AIChatVM : ObservableObject, IDockableVM
    {
        private string _title = "AI Assistant";
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        
        public bool IsTool => true;
        
        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }
        
        private bool _hasSettings = false;
        public bool HasSettings
        {
            get => _hasSettings;
            set => SetProperty(ref _hasSettings, value);
        }

        public string ContentId => "AIAssistant_Chat";
        
        private bool _canClose = true;
        public bool CanClose
        {
            get => _canClose;
            set => SetProperty(ref _canClose, value);
        }

        private bool _isClosed = false;
        public bool IsClosed
        {
            get => _isClosed;
            set => SetProperty(ref _isClosed, value);
        }

        private GeometryGroup? _imageGeometry;
        public GeometryGroup? ImageGeometry
        {
            get => _imageGeometry;
            set => SetProperty(ref _imageGeometry, value);
        }

        public ICommand? HideCommand { get; private set; }

        public void Hide(object? o)
        {
            IsVisible = false;
            IsClosed = true;
        }

        private string _userMessage = string.Empty;
        public string UserMessage
        {
            get => _userMessage;
            set => SetProperty(ref _userMessage, value);
        }

        private bool _isProcessing;
        public bool IsProcessing
        {
            get => _isProcessing;
            set => SetProperty(ref _isProcessing, value);
        }

        private string _statusMessage = "Ready - Enter your question below";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public ObservableCollection<ChatMessage> Messages { get; } = new();

        public ICommand SendMessageCommand { get; }
        public ICommand ClearChatCommand { get; }

        private readonly AIService? _aiService;
        private bool _mcpInitialized = false;

        public AIChatVM()
        {
            HideCommand = new CommunityToolkit.Mvvm.Input.RelayCommand<object?>(Hide);
            SendMessageCommand = new MvvmAsyncRelayCommand(SendMessageAsync);
            ClearChatCommand = new MvvmRelayCommand(ClearChat);
            
            // Get AIService from plugin instance
            _aiService = AIAssistantPlugin.Instance?.GetAIService();
            
            // Add welcome message
            var mcpEnabled = AIAssistantPlugin.Instance?.MCPEnabled ?? false;
            var welcomeMsg = "Hello! I'm your AI assistant for astrophotography. Ask me anything about:\n\n" +
                         "• Equipment settings and optimization\n" +
                         "• Target selection and planning\n" +
                         "• Image processing tips\n" +
                         "• Troubleshooting issues\n\n";
            
            if (mcpEnabled)
            {
                welcomeMsg += "🤖 **MCP Control Enabled** - I can directly control your NINA equipment!\n" +
                             "Try: \"Connect to NINA\", \"Show equipment status\", \"Take a 10s exposure\"\n\n";
            }
            
            welcomeMsg += "Make sure you've configured your API key in the plugin settings!";
            
            Messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = welcomeMsg,
                Timestamp = DateTime.Now
            });
        }

        private async Task InitializeMCPIfNeeded()
        {
            if (_mcpInitialized) 
            {
                Logger.Debug("AIChatVM: MCP already initialized, skipping");
                return;
            }
            
            var plugin = AIAssistantPlugin.Instance;
            if (plugin == null)
            {
                Logger.Warning("AIChatVM: Plugin instance is null, cannot initialize MCP");
                return;
            }
            
            Logger.Info($"AIChatVM: MCP Enabled setting: {plugin.MCPEnabled}");
            Logger.Info($"AIChatVM: Selected Provider: {plugin.SelectedProvider}");
            Logger.Info($"AIChatVM: AIService ActiveProviderType: {_aiService?.ActiveProviderType}");
            
            if (!plugin.MCPEnabled)
            {
                Logger.Info("AIChatVM: MCP is disabled in settings");
                return;
            }
            
            // Only enable MCP for Anthropic provider
            if (_aiService?.ActiveProviderType == AIProviderType.Anthropic)
            {
                var provider = _aiService.GetActiveProvider() as AnthropicProvider;
                if (provider != null)
                {
                    var mcpConfig = plugin.GetMCPConfig();
                    Logger.Info($"AIChatVM: Initializing MCP with config - Host: {mcpConfig.NinaHost}, Port: {mcpConfig.NinaPort}, Enabled: {mcpConfig.Enabled}");
                    
                    var success = await provider.EnableMCPAsync(mcpConfig);
                    _mcpInitialized = success;
                    
                    if (success)
                    {
                        Logger.Info("AIChatVM: MCP initialized successfully for Anthropic provider");
                        StatusMessage = "🤖 MCP Connected";
                    }
                    else
                    {
                        Logger.Warning("AIChatVM: MCP initialization failed - check NINA Advanced API connection");
                        StatusMessage = "⚠️ MCP connection failed";
                    }
                }
                else
                {
                    Logger.Warning("AIChatVM: Could not cast active provider to AnthropicProvider");
                }
            }
            else
            {
                Logger.Info($"AIChatVM: MCP not enabled - provider is {_aiService?.ActiveProviderType}, need Anthropic");
            }
        }

        private async Task SendMessageAsync()
        {
            if (string.IsNullOrWhiteSpace(UserMessage))
                return;

            // Check if the current provider has an API key configured
            var plugin = AIAssistantPlugin.Instance;
            if (plugin == null)
            {
                StatusMessage = "⚠️ Plugin not initialized";
                return;
            }

            // Validate API key based on selected provider
            var provider = plugin.SelectedProvider;
            bool hasValidKey = provider switch
            {
                AIProviderType.GitHub => !string.IsNullOrEmpty(plugin.GitHubApiKey),
                AIProviderType.OpenAI => !string.IsNullOrEmpty(plugin.OpenAIApiKey),
                AIProviderType.Anthropic => !string.IsNullOrEmpty(plugin.AnthropicApiKey),
                AIProviderType.Google => !string.IsNullOrEmpty(plugin.GoogleApiKey),
                AIProviderType.OpenRouter => !string.IsNullOrEmpty(plugin.OpenRouterApiKey),
                AIProviderType.Ollama => true, // Ollama doesn't need API key
                _ => false
            };

            if (!hasValidKey)
            {
                StatusMessage = $"⚠️ Please configure your {provider} API key in Options → Plugins";
                return;
            }

            if (_aiService == null)
            {
                StatusMessage = "⚠️ AI Service not initialized";
                return;
            }

            var userMsg = UserMessage;
            UserMessage = string.Empty;

            // Add user message to chat
            Messages.Add(new ChatMessage
            {
                Role = "user",
                Content = userMsg,
                Timestamp = DateTime.Now
            });

            IsProcessing = true;
            StatusMessage = "🤔 Thinking...";

            try
            {
                // Initialize MCP if needed (for Anthropic with MCP enabled)
                await InitializeMCPIfNeeded();
                
                // Build context based on whether MCP is enabled
                string? systemPrompt = null; // Let the provider use its own system prompt for MCP
                
                var mcpEnabled = plugin.MCPEnabled;
                var isAnthropic = _aiService?.ActiveProviderType == AIProviderType.Anthropic;
                
                // Only use generic system prompt if MCP is NOT enabled (let AnthropicProvider use its MCP prompt)
                if (!mcpEnabled || !isAnthropic)
                {
                    systemPrompt = @"You are an expert astrophotography assistant integrated into N.I.N.A. (Nighttime Imaging 'N' Astronomy) software. 
You help users with:
- Equipment setup and optimization (cameras, telescopes, mounts, filters)
- Imaging session planning (targets, exposure times, filter sequences)
- Image acquisition troubleshooting
- Processing workflows and techniques
- Weather and seeing conditions
Keep responses concise but informative. Use technical terms appropriately.";
                }

                var response = await _aiService.QueryAsync(userMsg, systemPrompt, CancellationToken.None);

                Messages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = response,
                    Timestamp = DateTime.Now
                });

                StatusMessage = "✓ Ready";
            }
            catch (Exception ex)
            {
                Logger.Error($"AI Query failed: {ex.Message}");
                Messages.Add(new ChatMessage
                {
                    Role = "assistant",
                    Content = $"Sorry, I encountered an error: {ex.Message}\n\nPlease check your API token and try again.",
                    Timestamp = DateTime.Now,
                    IsError = true
                });
                StatusMessage = "⚠️ Error - check your API token";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void ClearChat()
        {
            Messages.Clear();
            Messages.Add(new ChatMessage
            {
                Role = "assistant",
                Content = "Chat cleared. How can I help you?",
                Timestamp = DateTime.Now
            });
            StatusMessage = "Ready";
        }

        public void Dispose() { }
    }

    public class ChatMessage : ObservableObject
    {
        private string _role = string.Empty;
        public string Role
        {
            get => _role;
            set
            {
                SetProperty(ref _role, value);
                OnPropertyChanged(nameof(IsUser));
                OnPropertyChanged(nameof(IsAssistant));
            }
        }

        private string _content = string.Empty;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private DateTime _timestamp;
        public DateTime Timestamp
        {
            get => _timestamp;
            set => SetProperty(ref _timestamp, value);
        }

        private bool _isError;
        public bool IsError
        {
            get => _isError;
            set => SetProperty(ref _isError, value);
        }

        public bool IsUser => Role == "user";
        public bool IsAssistant => Role == "assistant";
    }
}
