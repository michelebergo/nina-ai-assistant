# Installation Guide

## Requirements

- **NINA** version 3.0 or later
- **.NET 8.0 Runtime** (included with NINA 3.x)
- **Internet connection** for AI providers (except Ollama)

## Installation Methods

### Method 1: NINA Plugin Manager (Recommended)

1. Open NINA
2. Go to **Options** → **Plugins**
3. Search for "**AI Assistant**"
4. Click **Install**
5. Restart NINA when prompted

### Method 2: Manual Installation

1. Download the latest release from the [Releases page](../../releases)
2. Extract the ZIP file
3. Copy all files to:
   ```
   %localappdata%\NINA\Plugins\3.0.0\AI Assistant\
   ```
4. Restart NINA

### Method 3: Build from Source

```bash
git clone https://github.com/michelebergo/NINA_AI_plugin.git
cd NINA_AI_plugin
dotnet build -c Release
```

The build automatically copies files to the NINA plugins folder.

## Post-Installation

1. Start NINA
2. Go to **Options** → **Plugins** → **AI Assistant**
3. Select your preferred AI provider
4. Enter your API key
5. Click **Test Connection**
6. Open the **AI Assistant** panel from the sidebar

## MCP Equipment Control (Optional)

To enable AI-controlled equipment:

1. Install the **NINA Advanced API** plugin from the Plugin Manager
2. In AI Assistant settings:
   - Select **Anthropic Claude (MCP)** as provider
   - Enable **MCP Control** checkbox
   - Set Host: `localhost`, Port: `1888`
3. Click **Test MCP Connection**

## Troubleshooting

### Plugin not appearing
- Ensure NINA is version 3.0 or later
- Check the plugins folder path is correct
- Restart NINA completely

### Connection test fails
- Verify your API key is correct
- Check your internet connection
- For Ollama, ensure the service is running

### MCP connection fails
- Verify NINA Advanced API plugin is installed and enabled
- Check port 1888 is not blocked by firewall
- Ensure NINA is running on the same machine (or configure remote host)

## Uninstallation

1. Go to **Options** → **Plugins**
2. Find **AI Assistant**
3. Click **Uninstall**
4. Restart NINA
