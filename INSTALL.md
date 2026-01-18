# Installation Guide - NINA AI Assistant

## Prerequisites

1. **N.I.N.A. 3.0 or later** installed
2. **.NET 8.0 Runtime** (usually installed with NINA 3.0+)
3. **Internet connection** for AI API access
4. **API key** for at least one AI provider

## Installation Methods

### Method 1: From NINA Plugin Manager (Recommended)

1. Open N.I.N.A.
2. Go to **Options** → **Equipment** → **Plugin Manager**
3. Click **Available Plugins** tab
4. Search for "AI Assistant"
5. Click **Install**
6. Restart N.I.N.A.

### Method 2: Manual Installation

1. Download the latest release from [GitHub Releases](https://github.com/michelebergo/nina-ai-assistant/releases)
2. Extract the zip file
3. Copy the contents to: `%LOCALAPPDATA%\NINA\Plugins\AI Assistant\`
4. Restart N.I.N.A.

### Method 3: Build from Source

```bash
# Clone the repository
git clone https://github.com/michelebergo/nina-ai-assistant.git
cd nina-ai-assistant

# Build the project
dotnet build -c Release

# The plugin will automatically copy to NINA plugins folder
# Or manually copy from: NINA.Plugin.AIAssistant\bin\Release\net8.0-windows\
```

## Initial Configuration

### Quick Setup (GitHub Models - FREE)

**Best for getting started!**

1. **Get GitHub Token**
   ```
   → Visit: https://github.com/settings/tokens
   → Click "Generate new token (classic)"
   → Name it: "NINA AI Assistant"
   → Expiration: Choose your preference (90 days recommended)
   → Scopes: No special scopes needed
   → Click "Generate token"
   → COPY THE TOKEN (you won't see it again!)
   ```

2. **Configure in NINA**
   ```
   → Open NINA
   → Options → Equipment → Plugin Manager
   → Click on "AI Assistant" plugin
   → In plugin settings:
      ✓ Check "Enable GitHub Models"
      ✓ Paste your GitHub token in "GitHub Personal Access Token"
      ✓ Select model: "gpt-4o-mini" (recommended to start)
      ✓ Set "Active Provider" to "GitHub Models (Free)"
   → Click anywhere outside to save
   ```

3. **Test It**
   ```
   → Go to Sequencer tab
   → Click "+" to add instruction
   → Find "AI Assistant" category
   → Add "AI Query Assistant"
   → Set question: "What is astrophotography?"
   → Click Run Sequence
   → You should see AI response in notification!
   ```

### Other Providers

<details>
<summary><b>Microsoft Foundry Setup</b></summary>

1. Go to https://ai.azure.com
2. Create a new project
3. Deploy a model (e.g., GPT-4o)
4. Get endpoint and API key from project settings
5. In NINA plugin settings:
   - Enable Microsoft Foundry
   - Enter API key
   - Enter endpoint URL
   - Enter deployment name
   - Set as active provider
</details>

<details>
<summary><b>OpenAI Setup</b></summary>

1. Go to https://platform.openai.com/api-keys
2. Click "Create new secret key"
3. Copy the key
4. In NINA plugin settings:
   - Enable OpenAI
   - Paste API key
   - Select model (e.g., gpt-4o-mini)
   - Set as active provider
</details>

<details>
<summary><b>Anthropic Claude Setup</b></summary>

1. Go to https://console.anthropic.com/
2. Create API key
3. Copy the key
4. In NINA plugin settings:
   - Enable Anthropic
   - Paste API key
   - Select model (e.g., claude-sonnet-4-5)
   - Set as active provider
</details>

<details>
<summary><b>Google AI Setup</b></summary>

1. Go to https://makersuite.google.com/app/apikey
2. Create API key
3. Copy the key
4. In NINA plugin settings:
   - Enable Google AI
   - Paste API key
   - Select model (e.g., gemini-2.0-flash)
   - Set as active provider
</details>

## Verification

### Test Your Setup

1. Open NINA Sequencer
2. Add a new instruction
3. Look for "AI Assistant" category - you should see:
   - ✅ AI Image Analysis
   - ✅ AI Suggest Exposure
   - ✅ AI Query Assistant

4. Add "AI Query Assistant"
5. Enter a simple question
6. Run the sequence
7. Check for AI response in notifications

If you see a response, **you're all set!** 🎉

## Common Issues

### "Provider not initialized"
- **Solution**: Check that provider is enabled and has valid API key
- Test connection in plugin settings

### "No AI provider configured"
- **Solution**: Enable at least one provider in plugin settings
- Make sure API key is entered

### Plugin doesn't appear
- **Solution**: 
  - Check NINA version (need 3.0+)
  - Check plugin is in `%LOCALAPPDATA%\NINA\Plugins\AI Assistant\`
  - Restart NINA completely
  - Check NINA logs for errors

### API key not working
- **Solution**:
  - Verify key is correct (no spaces)
  - Check key hasn't expired
  - Verify you have credits/quota (for paid providers)
  - Test with GitHub Models first (most forgiving)

## Next Steps

1. ✅ Install plugin
2. ✅ Configure provider
3. ✅ Test with simple query
4. 📖 Read the [README.md](README.md) for usage examples
5. 🌟 Start using AI in your sequences!

## Support

- 📚 [Full Documentation](README.md)
- 🐛 [Report Issues](https://github.com/michelebergo/nina-ai-assistant/issues)
- 💬 [NINA Discord](https://discord.gg/nighttime-imaging)

---

**Happy Imaging! 🔭✨**
