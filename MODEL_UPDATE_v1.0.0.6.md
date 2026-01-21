# Model Defaults Updated - v1.0.0.6

## Summary of Changes

All AI model defaults have been updated to the latest versions available as of January 2026.

## Updated Default Models

### **OpenAI** (previously: gpt-4o-mini)
- **New Default**: `gpt-4o`
- **Reason**: More capable flagship model, better quality responses
- **Fallback**: gpt-4o-mini still available in dropdown

### **GitHub Models** (previously: gpt-4o-mini)
- **New Default**: `gpt-4o`
- **Reason**: GitHub offers GPT-4o for free, best quality
- **Fallback**: gpt-4o-mini, o1-mini, Llama, Mistral still available

### **Google Gemini** (previously: gemini-1.5-flash)
- **New Default**: `gemini-2.0-flash-exp`
- **Reason**: Latest Gemini 2.0 generation with improved capabilities
- **Fallback**: gemini-1.5-flash, gemini-1.5-pro still available

### **Anthropic Claude** (unchanged)
- **Default**: `claude-sonnet-4-20250514`
- **Already latest**: Claude Sonnet 4 from May 2025

## Files Modified

1. **Settings.Designer.cs**
   - GitHub: `gpt-4o-mini` → `gpt-4o`
   - OpenAI: `gpt-4o-mini` → `gpt-4o`
   - Google: `gemini-1.5-flash` → `gemini-2.0-flash-exp`

2. **AIAssistantPlugin.cs**
   - Updated all fallback values to new defaults
   - Updated SanitizeModelId() method defaults

3. **Models.cs**
   - Reordered model lists to show new defaults first
   - Added "(default)" labels for clarity

4. **Options.xaml.cs**
   - Updated GitHub test model reference

## Impact on Users

### **New Installations**
- Will get the latest, most capable models by default
- Better out-of-box experience

### **Existing Users**
- **No automatic change** - your saved model selection is preserved
- **To upgrade**: Go to Options → Plugins → AI Assistant → Model dropdown → Select new model
- **Or**: Delete your settings to reset to new defaults

### **Cost Implications**

**GitHub Models**: ✅ **FREE** - All models including GPT-4o
**OpenAI Direct**: ⚠️ **Higher cost** - GPT-4o is ~3x more expensive than gpt-4o-mini
  - Input: $2.50/1M tokens (vs $0.15/1M for mini)
  - Output: $10/1M tokens (vs $0.60/1M for mini)
**Google Gemini**: ✅ **FREE tier** - 2.0 Flash still in free tier
**Anthropic**: ⚠️ **Premium** - Claude Sonnet 4 is higher tier pricing

### **Performance Comparison**

| Model | Quality | Speed | Cost | Use Case |
|-------|---------|-------|------|----------|
| **gpt-4o** | ⭐⭐⭐⭐⭐ | ⚡⚡⚡⚡ | 💰💰💰 | Best quality, complex tasks |
| gpt-4o-mini | ⭐⭐⭐⭐ | ⚡⚡⚡⚡⚡ | 💰 | Fast, simple tasks |
| **gemini-2.0-flash-exp** | ⭐⭐⭐⭐⭐ | ⚡⚡⚡⚡⚡ | ✅ Free | Latest Google, experimental |
| gemini-1.5-flash | ⭐⭐⭐⭐ | ⚡⚡⚡⚡⚡ | ✅ Free | Stable, production |
| **claude-sonnet-4** | ⭐⭐⭐⭐⭐ | ⚡⚡⚡⚡ | 💰💰💰💰 | Best reasoning, writing |

## Recommendations

### **For Free Usage**: 
Use **GitHub Models** (GPT-4o free!) or **Google Gemini** (2.0 Flash free tier)

### **For Best Quality**:
Use **Claude Sonnet 4** or **GPT-4o**

### **For Speed/Cost**:
Use **gpt-4o-mini** or **gemini-1.5-flash**

### **For MCP/Tools**:
Use **Gemini 2.0** or **Claude Sonnet 4** (best function calling support)

## Migration Path

If you want to stick with cheaper models:
1. Open NINA
2. Go to Options → Plugins → AI Assistant
3. Change model dropdown to:
   - `gpt-4o-mini` (OpenAI/GitHub)
   - `gemini-1.5-flash` (Google)
4. Save settings

## Testing Recommendations

After update, test with your provider:
1. Simple query: "What version of NINA am I running?"
2. Complex query: "Plan an imaging session for M31 tonight with my equipment"
3. MCP tool test: "Connect to my camera and check its temperature"

All models support these features, but quality/speed may vary.

## Build Status

✅ **Build Successful**
✅ **All providers updated**
✅ **Backward compatible** (old models still available)
✅ **Settings preserved** (won't override user choices)
