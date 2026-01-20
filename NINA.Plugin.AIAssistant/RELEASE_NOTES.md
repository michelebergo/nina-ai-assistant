# AI Assistant Plugin v1.0.0.3 - Release Notes

## 🎉 First Release

### Features
- **Multi-Provider AI Support**: Connect to GitHub Models, OpenAI, Anthropic, Google, Ollama, or OpenRouter
- **NINA Integration**: Dockable panel accessible from Tools menu with chat bubble icon
- **MCP Control**: Equipment control via Model Context Protocol when available
- **Chat Interface**: Clean, intuitive chat interface with message history
- **Provider Selection**: Easy dropdown to switch between AI providers
- **API Key Management**: Secure configuration for each provider in Settings

### Technical Details
- **Version**: 1.0.0.3
- **Minimum NINA Version**: 3.0.0.0
- **Framework**: .NET 8.0
- **License**: MIT

### Installation
1. Extract `AIAssistant-1.0.0.3.zip` to `%localappdata%\NINA\Plugins\3.0.0\AI Assistant\`
2. Restart NINA
3. Configure API keys in Options → Equipment → AI Assistant
4. Access the panel via the chat bubble icon in the Tools area (top right)

### Supported AI Providers
- **GitHub Models**: Free tier available with GitHub account
- **OpenAI**: GPT-4o, GPT-4, GPT-3.5-turbo
- **Anthropic**: Claude 3.5 Sonnet, Claude 3 Opus, Claude 3 Haiku
- **Google**: Gemini 1.5 Pro, Gemini 1.5 Flash
- **Ollama**: Local LLM hosting (requires Ollama running locally)
- **OpenRouter**: Access to multiple models via single API

### UI Layout
- 350px fixed-width input area
- Clear message display with role indicators (User/Assistant)
- Send and Clear buttons for easy interaction

### Known Limitations
- MCP functionality requires compatible server setup
- Provider availability depends on API key configuration

### Credits
Developed by Michele Bergo
Repository: https://github.com/michelebergo/nina-ai-assistant

---

**For issues or feature requests, please visit the GitHub repository.**
