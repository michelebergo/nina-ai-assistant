using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Azure.AI.Inference;
using Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NINA.Plugin.AIAssistant.AI;

namespace NINA.Plugin.AIAssistant
{
    [Export(typeof(ResourceDictionary))]
    public partial class Options : ResourceDictionary
    {
        public Options()
        {
            InitializeComponent();
        }

        #region Provider Selection

        private void ProviderSelector_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.DataContext is AIAssistantPlugin plugin)
            {
                // Set the selected item based on the saved provider
                var savedProvider = plugin.SelectedProvider.ToString();
                foreach (ComboBoxItem item in comboBox.Items)
                {
                    if (item.Tag?.ToString() == savedProvider)
                    {
                        comboBox.SelectedItem = item;
                        break;
                    }
                }
                
                // Update status text
                UpdateProviderStatus(comboBox, plugin);
            }
        }

        private void ProviderSelector_Changed(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.SelectedItem is ComboBoxItem selectedItem && 
                comboBox.DataContext is AIAssistantPlugin plugin)
            {
                var providerTag = selectedItem.Tag?.ToString();
                if (!string.IsNullOrEmpty(providerTag) && 
                    Enum.TryParse<AIProviderType>(providerTag, out var providerType))
                {
                    plugin.SelectedProvider = providerType;
                    UpdateProviderStatus(comboBox, plugin);
                }
            }
        }

        private void UpdateProviderStatus(ComboBox comboBox, AIAssistantPlugin plugin)
        {
            var statusText = FindTextBlock("ProviderStatusText", comboBox);
            if (statusText == null) return;

            var provider = plugin.SelectedProvider;
            string status = provider switch
            {
                AIProviderType.Anthropic when plugin.MCPEnabled => "✓ MCP-enabled for NINA control",
                AIProviderType.Anthropic => "Claude AI (enable MCP for equipment control)",
                AIProviderType.GitHub => "Using GitHub-hosted models",
                AIProviderType.OpenAI => "Using OpenAI API",
                AIProviderType.Google => "Using Google Gemini",
                AIProviderType.Ollama => "Local AI models",
                AIProviderType.OpenRouter => "Multiple providers via OpenRouter",
                _ => ""
            };
            statusText.Text = status;
        }

        #endregion

        #region GitHub Token Handlers

        private void GitHubToken_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin)
            {
                plugin.GitHubApiKey = passwordBox.Password;
            }
            ClearTestResult("GitHubTestResult", sender);
        }

        private void GitHubToken_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin && !string.IsNullOrEmpty(plugin.GitHubApiKey))
            {
                passwordBox.Password = plugin.GitHubApiKey;
            }
        }

        private void GetGitHubToken_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/settings/tokens/new",
                UseShellExecute = true
            });
        }

        private async void TestGitHubKey_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("GitHubTestResult", button);
            if (resultTextBlock == null) return;

            if (string.IsNullOrWhiteSpace(plugin.GitHubApiKey))
            {
                ShowResult(resultTextBlock, "⚠️ Please enter an API token first", Colors.Orange);
                return;
            }

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing API key...", Colors.White);

            try
            {
                var endpoint = new Uri("https://models.inference.ai.azure.com");
                var credential = new AzureKeyCredential(plugin.GitHubApiKey);
                var client = new ChatCompletionsClient(endpoint, credential);

                var response = await client.CompleteAsync(new ChatCompletionsOptions
                {
                    Model = plugin.GitHubModelId ?? "gpt-4o-mini",
                    Messages = { new ChatRequestUserMessage("Say 'OK'") },
                    MaxTokens = 5
                });

                ShowResult(resultTextBlock, $"✅ GitHub API key is valid!", Colors.LightGreen);
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region OpenAI Token Handlers

        private void OpenAIToken_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin)
            {
                plugin.OpenAIApiKey = passwordBox.Password;
            }
            ClearTestResult("OpenAITestResult", sender);
        }

        private void OpenAIToken_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin && !string.IsNullOrEmpty(plugin.OpenAIApiKey))
            {
                passwordBox.Password = plugin.OpenAIApiKey;
            }
        }

        private void GetOpenAIKey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://platform.openai.com/api-keys",
                UseShellExecute = true
            });
        }

        private async void TestOpenAIKey_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("OpenAITestResult", button);
            if (resultTextBlock == null) return;

            if (string.IsNullOrWhiteSpace(plugin.OpenAIApiKey))
            {
                ShowResult(resultTextBlock, "⚠️ Please enter an API key first", Colors.Orange);
                return;
            }

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing API key...", Colors.White);

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plugin.OpenAIApiKey);
                
                var response = await client.GetAsync("https://api.openai.com/v1/models");
                
                if (response.IsSuccessStatusCode)
                {
                    ShowResult(resultTextBlock, "✅ OpenAI API key is valid!", Colors.LightGreen);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    ShowResult(resultTextBlock, $"❌ Invalid API key: {response.StatusCode}", Colors.Salmon);
                }
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region Anthropic Token Handlers

        private void AnthropicToken_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin)
            {
                plugin.AnthropicApiKey = passwordBox.Password;
            }
            ClearTestResult("AnthropicTestResult", sender);
        }

        private void AnthropicToken_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin && !string.IsNullOrEmpty(plugin.AnthropicApiKey))
            {
                passwordBox.Password = plugin.AnthropicApiKey;
            }
        }

        private void GetAnthropicKey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://console.anthropic.com/settings/keys",
                UseShellExecute = true
            });
        }

        private async void TestAnthropicKey_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("AnthropicTestResult", button);
            if (resultTextBlock == null) return;

            if (string.IsNullOrWhiteSpace(plugin.AnthropicApiKey))
            {
                ShowResult(resultTextBlock, "⚠️ Please enter an API key first", Colors.Orange);
                return;
            }

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing API key...", Colors.White);

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("x-api-key", plugin.AnthropicApiKey);
                client.DefaultRequestHeaders.Add("anthropic-version", "2023-06-01");

                var requestBody = new
                {
                    model = "claude-3-5-haiku-20241022",
                    max_tokens = 5,
                    messages = new[] { new { role = "user", content = "Say OK" } }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("https://api.anthropic.com/v1/messages", content);

                if (response.IsSuccessStatusCode)
                {
                    ShowResult(resultTextBlock, "✅ Anthropic API key is valid!", Colors.LightGreen);
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ShowResult(resultTextBlock, $"❌ Invalid API key: {response.StatusCode}", Colors.Salmon);
                }
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region Google Token Handlers

        private void GoogleToken_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin)
            {
                plugin.GoogleApiKey = passwordBox.Password;
            }
            ClearTestResult("GoogleTestResult", sender);
        }

        private void GoogleToken_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin && !string.IsNullOrEmpty(plugin.GoogleApiKey))
            {
                passwordBox.Password = plugin.GoogleApiKey;
            }
        }

        private void GetGoogleKey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://aistudio.google.com/app/apikey",
                UseShellExecute = true
            });
        }

        private async void TestGoogleKey_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("GoogleTestResult", button);
            if (resultTextBlock == null) return;

            if (string.IsNullOrWhiteSpace(plugin.GoogleApiKey))
            {
                ShowResult(resultTextBlock, "⚠️ Please enter an API key first", Colors.Orange);
                return;
            }

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing API key...", Colors.White);

            try
            {
                using var client = new HttpClient();
                
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = "Say OK" } } } },
                    generationConfig = new { maxOutputTokens = 5 }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash:generateContent?key={plugin.GoogleApiKey}";
                var response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    ShowResult(resultTextBlock, "✅ Google API key is valid!", Colors.LightGreen);
                }
                else
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    ShowResult(resultTextBlock, $"❌ Invalid API key: {response.StatusCode}", Colors.Salmon);
                }
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region Ollama Handlers

        private void GetOllama_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://ollama.ai/download",
                UseShellExecute = true
            });
        }

        private async void TestOllama_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("OllamaTestResult", button);
            if (resultTextBlock == null) return;

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing Ollama connection...", Colors.White);

            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                
                var endpoint = plugin.OllamaEndpoint ?? "http://localhost:11434";
                var response = await client.GetAsync($"{endpoint}/api/tags");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var json = JObject.Parse(content);
                    var models = json["models"]?.ToObject<JArray>();
                    var modelCount = models?.Count ?? 0;
                    
                    ShowResult(resultTextBlock, $"✅ Ollama is running! {modelCount} model(s) available.", Colors.LightGreen);
                }
                else
                {
                    ShowResult(resultTextBlock, $"❌ Ollama responded with: {response.StatusCode}", Colors.Salmon);
                }
            }
            catch (HttpRequestException)
            {
                ShowResult(resultTextBlock, "❌ Cannot connect. Make sure Ollama is running.", Colors.Salmon);
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region OpenRouter Token Handlers

        private void OpenRouterToken_Changed(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin)
            {
                plugin.OpenRouterApiKey = passwordBox.Password;
            }
            ClearTestResult("OpenRouterTestResult", sender);
        }

        private void OpenRouterToken_Loaded(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            if (passwordBox?.DataContext is AIAssistantPlugin plugin && !string.IsNullOrEmpty(plugin.OpenRouterApiKey))
            {
                passwordBox.Password = plugin.OpenRouterApiKey;
            }
        }

        private void GetOpenRouterKey_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://openrouter.ai/keys",
                UseShellExecute = true
            });
        }

        private async void TestOpenRouterKey_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("OpenRouterTestResult", button);
            if (resultTextBlock == null) return;

            if (string.IsNullOrWhiteSpace(plugin.OpenRouterApiKey))
            {
                ShowResult(resultTextBlock, "⚠️ Please enter an API key first", Colors.Orange);
                return;
            }

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing API key...", Colors.White);

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", plugin.OpenRouterApiKey);
                
                var response = await client.GetAsync("https://openrouter.ai/api/v1/auth/key");
                
                if (response.IsSuccessStatusCode)
                {
                    ShowResult(resultTextBlock, "✅ OpenRouter API key is valid!", Colors.LightGreen);
                }
                else
                {
                    ShowResult(resultTextBlock, $"❌ Invalid API key: {response.StatusCode}", Colors.Salmon);
                }
            }
            catch (Exception ex)
            {
                HandleApiError(resultTextBlock, ex);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        #endregion

        #region MCP Handlers

        private async void TestMCPConnection_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext is not AIAssistantPlugin plugin) return;

            var resultTextBlock = FindTextBlock("MCPTestResult", button);
            if (resultTextBlock == null) return;

            button.IsEnabled = false;
            ShowResult(resultTextBlock, "🔄 Testing NINA Advanced API connection...", Colors.White);

            try
            {
                var mcpConfig = plugin.GetMCPConfig();
                var client = new AI.MCP.NINAAdvancedAPIClient();
                var connected = await client.InitializeAsync(mcpConfig);

                if (connected)
                {
                    ShowResult(resultTextBlock, $"✅ Connected to NINA Advanced API at {mcpConfig.NinaHost}:{mcpConfig.NinaPort}", Colors.LightGreen);
                }
                else
                {
                    ShowResult(resultTextBlock, "❌ Could not connect. Ensure NINA is running with Advanced API plugin enabled.", Colors.Salmon);
                }
                
                client.Close();
            }
            catch (Exception ex)
            {
                ShowResult(resultTextBlock, $"❌ Connection failed: {ex.Message}", Colors.Salmon);
            }
            finally
            {
                button.IsEnabled = true;
            }
        }

        private void GetAdvancedAPIPlugin_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/christian-photo/ninaAPI",
                UseShellExecute = true
            });
        }

        private void GetMCPRepo_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/michelebergo/nina_mcp_server",
                UseShellExecute = true
            });
        }

        #endregion

        #region Helper Methods

        private TextBlock? FindTextBlock(string name, FrameworkElement startElement)
        {
            var parent = startElement.Parent as FrameworkElement;
            while (parent != null)
            {
                if (parent is StackPanel stackPanel)
                {
                    foreach (var child in stackPanel.Children)
                    {
                        if (child is TextBlock tb && tb.Name == name)
                        {
                            return tb;
                        }
                    }
                }
                parent = parent.Parent as FrameworkElement;
            }
            return null;
        }

        private void ClearTestResult(string textBlockName, object sender)
        {
            if (sender is FrameworkElement element)
            {
                var tb = FindTextBlock(textBlockName, element);
                if (tb != null)
                {
                    tb.Text = "";
                }
            }
        }

        private void ShowResult(TextBlock textBlock, string message, Color color)
        {
            textBlock.Foreground = new SolidColorBrush(color);
            textBlock.Text = message;
        }

        private void HandleApiError(TextBlock resultTextBlock, Exception ex)
        {
            if (ex.Message.Contains("401") || ex.Message.Contains("Unauthorized"))
            {
                ShowResult(resultTextBlock, "❌ Invalid API key. Please check your key.", Colors.Salmon);
            }
            else if (ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase))
            {
                ShowResult(resultTextBlock, "❌ Connection timeout. Server may be unavailable.", Colors.Salmon);
            }
            else
            {
                ShowResult(resultTextBlock, $"❌ Error: {ex.Message}", Colors.Salmon);
            }
        }

        #endregion
    }
}
