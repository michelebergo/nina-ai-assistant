# AI Assistant v1.0.0.1 - Initial Release

## 🎉 Welcome to AI Assistant for NINA!

AI Assistant brings powerful multi-provider AI capabilities directly into NINA, your astrophotography imaging suite. Chat with AI models and control your equipment through natural language using the Model Context Protocol (MCP).

## ✨ Key Features

### 🤖 Multi-Provider AI Support
Choose from 6 different AI providers:
- **GitHub Models** - Free tier available with GPT-4o
- **OpenAI** - GPT-4o, GPT-4o-mini
- **Anthropic Claude** - With MCP equipment control support
- **Google Gemini** - Free tier available
- **Ollama** - Run models locally (completely free)
- **OpenRouter** - Access to multiple models via pay-per-use

### 🔧 MCP Equipment Control
When using Anthropic Claude, you get direct equipment control through natural language:
- **Camera**: Connect, capture images, set cooling, abort exposures
- **Mount**: Slew to targets, park/unpark, tracking modes
- **Focuser**: Move to positions, temperature compensation
- **Filter Wheel**: Change filters automatically
- **Guider**: Start/stop guiding, run calibration
- **Dome**: Open/close shutter, park dome
- **And more**: 50+ equipment control tools via NINA Advanced API

### 💬 Interactive Chat Interface
- Dockable chat panel integrated into NINA's Equipment tab
- Real-time streaming responses
- Provider selection with status indicators
- Secure API key storage

## 🚀 Getting Started

1. **Install the plugin**: Place `NINA.Plugin.AIAssistant.dll` in your NINA plugins folder
2. **Enable NINA Advanced API**: 
   - Open NINA
   - Go to Options > Equipment > Advanced API
   - Check "Enable Advanced API"
   - Note the port (default: 1888)
3. **Configure AI Provider**:
   - Open AI Assistant options
   - Select your preferred provider
   - Enter your API key
   - (For MCP) Verify Advanced API connection
4. **Start chatting**: Open the AI Assistant panel and start asking questions!

## 📋 Requirements

- NINA 3.0 or later
- .NET 8.0 Runtime (included with NINA 3.x)
- For MCP features: NINA Advanced API enabled

## 🔐 Privacy & Security

- API keys are stored locally in NINA's user settings
- No data is sent to external servers except to your chosen AI provider
- Your settings never leave your computer

## 🐛 Known Issues

- Icon may not display during local development (will show once manifest is published)
- Some AI providers may have rate limits on free tiers

## 📝 Changelog

See [CHANGELOG.md](https://github.com/michelebergo/nina-ai-assistant/blob/main/CHANGELOG.md) for detailed changes.

## 🙏 Acknowledgments

Built with:
- [NINA](https://nighttime-imaging.eu/) - Nighttime Imaging 'N' Astronomy
- Anthropic Claude API for MCP support
- OpenAI, Google, and other AI provider APIs
- Microsoft.Extensions.AI for unified AI abstractions

## 📄 License

This plugin is licensed under the [Mozilla Public License 2.0](https://www.mozilla.org/en-US/MPL/2.0/).

---

**Author**: Michele Bergo  
**Repository**: https://github.com/michelebergo/nina-ai-assistant  
**Support**: Please open an issue on GitHub
