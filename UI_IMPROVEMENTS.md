# UI Improvements & Dynamic Model Loading

## Changes Made

### 1. Dynamic Model Discovery
All providers now dynamically fetch their available models from APIs:

- **OpenAI**: Queries `https://api.openai.com/v1/models` for GPT/O-series models
- **Google**: Queries Gemini API for models with `generateContent` capability  
- **GitHub Models**: Queries Azure AI inference endpoint for live model list
- **Anthropic**: Validates API key, returns curated Claude model list
- **Ollama**: Already had dynamic discovery via `/api/tags`

Each provider falls back to default hardcoded lists if API calls fail.

### 2. Simplified UI with Provider-Specific Visibility
- Only the selected provider's settings are now visible
- When you select "GitHub Models", only GitHub settings appear
- When you select "Anthropic Claude", only Anthropic + MCP settings appear
- Reduces UI clutter and focuses user attention on relevant settings

## How It Works

**Provider Selection**:
- `SelectedProviderInternal` property triggers UI visibility updates
- Each provider has an `IsXxxSelected` boolean property
- XAML uses `BooleanToVisibilityConverter` to show/hide GroupBoxes

**Model Loading**:
- Each model ComboBox has a `Loaded` event handler
- When Options dialog opens, models are fetched from provider APIs
- Models populate dynamically in dropdown
- Current saved model is pre-selected

## Testing
1. Open NINA → Options → AI Assistant
2. Select different providers - notice only that provider's settings appear
3. Click model dropdown - you should see all available models from the API
4. GitHub Models should show many more models than before (gpt-5, o1, o3-mini, etc.)

## Benefits
- **Always Up-to-Date**: New models appear automatically without code changes
- **Cleaner UI**: Less overwhelming for users, especially beginners
- **Better UX**: Focus on one provider at a time
- **Future-Proof**: When GPT-5 or Claude 5 releases, they'll appear automatically
