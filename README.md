# NINA AI Assistant Plugin

AI-powered features for [N.I.N.A. (Nighttime Imaging 'N' Astronomy)](https://nighttime-imaging.eu/) - bringing intelligent automation to your astrophotography workflow.

## 🌟 Features

### Multi-Provider AI Support
Support for both **free** and **paid** AI models from multiple providers:

#### Free Tier Available
- **GitHub Models** - Free tier with models like GPT-4o-mini, Phi-4, Llama 3.3, Claude Sonnet 4.5
- **Google AI** - Generous free tier with Gemini models

#### Paid APIs
- **Microsoft Foundry** (formerly Azure AI Foundry) - Enterprise-grade deployment
- **Azure OpenAI** - Fully managed OpenAI on Azure
- **OpenAI** - Direct API access to latest models
- **Anthropic Claude** - Advanced reasoning capabilities

### AI-Powered Sequence Items

1. **AI Image Analysis**
   - Automated quality assessment
   - Star detection and tracking evaluation
   - Focus quality analysis
   - Recommendations for improvement

2. **AI Suggest Exposure**
   - Intelligent exposure time recommendations
   - Target-specific guidance
   - Sky condition considerations
   - Sub-exposure count suggestions

3. **AI Query Assistant**
   - Ask any astrophotography question
   - Get expert advice in real-time
   - Learn best practices
   - Troubleshooting assistance

## 📋 Requirements

- N.I.N.A. version 3.0 or later
- .NET 8.0 Runtime
- At least one configured AI provider (see Setup)

## 🚀 Quick Start

### Option 1: GitHub Models (Recommended for Getting Started) - FREE!

1. **Get a GitHub Personal Access Token**
   - Visit https://github.com/settings/tokens
   - Click "Generate new token (classic)"
   - Select scopes: Just basic access is fine
   - Copy your token

2. **Configure in NINA**
   - Open NINA → Options → Equipment → Plugin Manager
   - Find "AI Assistant" plugin
   - Enable GitHub Models
   - Paste your GitHub PAT
   - Select a model (e.g., "gpt-4o-mini")
   - Click Test Connection

3. **Start Using**
   - Go to Sequencer
   - Add AI sequence items from "AI Assistant" category
   - Run your sequence!

### Option 2: Other Providers

#### Microsoft Foundry
1. Create a Foundry project at https://ai.azure.com
2. Deploy a model
3. Copy your endpoint and API key
4. Configure in plugin settings

#### OpenAI
1. Get API key from https://platform.openai.com/api-keys
2. Configure in plugin settings
3. Select your preferred model

#### Anthropic Claude
1. Get API key from https://console.anthropic.com/
2. Configure in plugin settings
3. Select Claude model

#### Google AI (Gemini)
1. Get API key from https://makersuite.google.com/app/apikey
2. Configure in plugin settings
3. Select Gemini model

## 💡 Usage Examples

### Example 1: Quality Check Sequence
```
1. Take Exposure (Light, 300s)
2. AI Image Analysis
   - Prompt: "Analyze image quality, focus, and tracking. Rate from 1-10 and suggest improvements."
3. Continue if quality is good, retry if issues found
```

### Example 2: Smart Exposure Planning
```
1. AI Suggest Exposure
   - Target: M42
   - Filter: Ha
   - Sky Brightness: 20.5
2. Use recommended settings for capture
```

### Example 3: Interactive Learning
```
1. AI Query Assistant
   - Question: "What's the best way to image faint galaxies with my setup?"
2. Follow AI recommendations
```

## 🎯 Supported Models

### GitHub Models (Free Tier Available)
- gpt-4o, gpt-4o-mini (OpenAI)
- gpt-4.1, gpt-4.1-mini (OpenAI)
- o1, o1-mini, o3-mini (OpenAI reasoning models)
- claude-sonnet-4-5 (Anthropic)
- llama-3.3-70b-instruct (Meta)
- phi-4 (Microsoft)

### Microsoft Foundry
- gpt-5.2, gpt-5.1 (OpenAI latest)
- claude-opus-4-5, claude-sonnet-4-5 (Anthropic)
- o3, o1 (OpenAI reasoning)
- All Foundry catalog models

### Azure OpenAI
- gpt-4o, gpt-4o-mini
- gpt-4, gpt-3.5-turbo

### OpenAI Direct
- gpt-4o, gpt-4o-mini
- gpt-4-turbo, gpt-4
- o1-preview, o1-mini

### Anthropic
- claude-opus-4-5 (Most capable)
- claude-sonnet-4-5 (Balanced)
- claude-haiku-4-5 (Fastest)

### Google AI
- gemini-2.0-flash (Latest)
- gemini-1.5-pro, gemini-1.5-flash

## ⚙️ Configuration

All settings are in NINA → Options → Equipment → Plugin Manager → AI Assistant:

- **Active Provider**: Which AI service to use
- **Provider Settings**: API keys, endpoints, model selection
- **Multiple Providers**: Configure several, switch as needed

## 🔒 Security & Privacy

- API keys are stored in NINA's secure profile settings
- No data is stored or logged by the plugin beyond NINA's normal logging
- All AI requests go directly to your chosen provider
- Review each provider's privacy policy and terms of service

## 💰 Cost Considerations

### Free Options
- **GitHub Models**: Free tier with rate limits, upgrade for production
- **Google AI**: Generous free tier for Gemini models

### Paid Options
Typical costs (January 2026):
- **GPT-4o-mini**: ~$0.26 per 1M tokens (very affordable)
- **GPT-4o**: ~$4.38 per 1M tokens
- **Claude Sonnet**: ~$6 per 1M tokens
- **Gemini Pro**: Varies by usage

💡 **Tip**: Start with GitHub Models or Gemini free tier, then upgrade if needed!

## 🐛 Troubleshooting

### "Provider not initialized" error
- Check that you've enabled the provider in settings
- Verify API key is correct
- Test connection in settings page

### "No AI provider configured" error
- Configure at least one provider in plugin settings
- Restart NINA after configuration

### High latency responses
- Consider using faster models (e.g., gpt-4o-mini, claude-haiku)
- Check your internet connection
- Some reasoning models (o1, o3) are slower by design

## 📝 Development

### Building from Source
```bash
git clone https://github.com/yourusername/nina-ai-assistant
cd nina-ai-assistant
dotnet build
```

### Contributing
Contributions welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Submit a pull request

## 📄 License

This project is licensed under the Mozilla Public License 2.0 (MPL-2.0).

## 🙏 Credits

- N.I.N.A. Team for the amazing imaging software
- All AI providers for their excellent APIs
- Community contributors

## 📞 Support

- [GitHub Issues](https://github.com/yourusername/nina-ai-assistant/issues)
- [N.I.N.A. Discord](https://discord.gg/nighttime-imaging)

## 🔗 Links

- [N.I.N.A. Official Website](https://nighttime-imaging.eu/)
- [Plugin Repository](https://github.com/yourusername/nina-ai-assistant)
- [GitHub Models](https://github.com/marketplace/models)
- [Microsoft Foundry](https://ai.azure.com)

---

**Made with ❤️ for the astrophotography community**
