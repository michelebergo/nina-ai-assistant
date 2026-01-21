# NINA AI Assistant - MCP Integration Summary

## 🎯 Overview

The NINA AI Assistant plugin now has **full Model Context Protocol (MCP) support** for both **Claude (Anthropic)** and **Gemini (Google)** providers, enabling direct equipment control through natural language commands.

## ✅ Completed Features

### 1. **Google Gemini MCP Support (NEW)**
- ✅ Added MCP function calling support to GoogleProvider
- ✅ Integrated with NINA Advanced API via NINAAdvancedAPIClient
- ✅ Supports all Gemini 2.0 models (gemini-2.0-flash-exp, gemini-exp-1206, etc.)
- ✅ Full function declaration conversion for Gemini's API format
- ✅ Multi-turn conversation support with function results
- ✅ Enhanced logging with [MCP] prefix for debugging

### 2. **Anthropic Claude MCP Support (ENHANCED)**
- ✅ Existing tool-use API integration maintained
- ✅ Works with Claude Sonnet 4 and Claude 3.5 models
- ✅ Full equipment control via NINA Advanced API
- ✅ Tool iteration loop with safety limits

### 3. **Unified MCP Architecture**
- ✅ Both providers use the same NINAAdvancedAPIClient
- ✅ Automatic MCP initialization in AIChatVM
- ✅ Provider-specific status messages ("MCP Connected (Claude)" / "MCP Connected (Gemini)")
- ✅ Seamless switching between providers with MCP enabled

## 🔧 Technical Implementation

### Google Gemini Function Calling

**Function Declaration Format:**
```json
{
  "name": "nina_get_status",
  "description": "Get the current status of all connected equipment",
  "parameters": {
    "type": "object",
    "properties": {},
    "required": []
  }
}
```

**Request Structure:**
```json
{
  "contents": [{"role": "user", "parts": [{"text": "check equipment"}]}],
  "tools": [{"function_declarations": [...]}],
  "generationConfig": {"temperature": 0.7, "maxOutputTokens": 8192}
}
```

**Function Response:**
```json
{
  "role": "function",
  "parts": [{
    "functionResponse": {
      "name": "nina_get_status",
      "response": {"content": "Camera: Connected, Cooled to -10°C..."}
    }
  }]
}
```

### Anthropic Claude Tool Use

**Tool Definition Format:**
```json
{
  "name": "nina_get_status",
  "description": "Get the current status of all connected equipment",
  "input_schema": {
    "type": "object",
    "properties": {},
    "required": []
  }
}
```

## 🚀 Available MCP Tools

### Equipment Control (40+ tools)
- **Camera**: connect, disconnect, capture, cooling, abort, statistics
- **Mount**: connect, disconnect, slew, park, unpark, home, tracking, sync
- **Focuser**: connect, disconnect, move, temperature compensation, autofocus
- **Filter Wheel**: connect, disconnect, change filter, get filter info
- **Guider**: start/stop guiding, PHD2 control, calibration, settle
- **Dome**: connect, disconnect, slew, park, open/close shutter
- **Rotator**: connect, disconnect, move, sync, reverse
- **Flat Panel**: connect, disconnect, light control, cover control
- **Weather**: get environmental data
- **Safety**: check safety status
- **Switches**: power box control

### Advanced Features
- **Sequences**: load, start, stop, edit
- **Plate Solving**: sync, center, blind solve
- **Framing Assistant**: coordinate setting, moon separation
- **Image Operations**: solve, get prepared image, statistics

## 📖 Usage Examples

### Gemini with MCP
```
User: "check equipment status"
Gemini: [Calls nina_get_status function]
       → "Your equipment is connected: Camera is cooled to -10°C, 
          Mount is tracking at RA 12h 34m, Focuser at position 2500"

User: "take a 60 second image"
Gemini: [Calls nina_capture_image with duration=60]
       → "Capturing 60 second exposure... Capture complete! 
          Image shows 1234 stars with HFR 2.3"
```

### Claude with MCP
```
User: "slew to M31"  
Claude: [Calls nina_slew_mount with ra=0.712, dec=41.27]
        → "Slewing mount to M31 (Andromeda Galaxy)... 
           Mount arrived at target coordinates"

User: "start guiding"
Claude: [Calls nina_start_guiding]
        → "Starting PHD2 guiding... Calibration complete, 
           guiding active with RMS error 0.45"
```

## 🔗 Integration with External MCP Server

The plugin can work alongside the external **[nina_mcp_server](https://github.com/michelebergo/nina_mcp_server)** Python MCP server:

### Architecture Options

1. **Built-in MCP (Current Implementation)**
   - Direct HTTP calls from plugin to NINA Advanced API
   - Works with Claude and Gemini inside NINA
   - Best for: Using AI within NINA interface

2. **External MCP Server (For External AI Apps)**
   - Python MCP server (fastmcp) running separately
   - AI agents (Claude Desktop, etc.) connect via MCP protocol
   - Works with: localhost:1888 NINA Advanced API
   - Best for: Using Claude Desktop or other external AI tools

### Future Enhancement: Hybrid Mode
Could add option to use external MCP server from within plugin:
- Launch nina_advanced_mcp.py as subprocess
- Connect both Gemini and Claude via MCP protocol instead of direct HTTP
- Benefit: 100% API coverage (150/150 endpoints from MCP server)

## 🛠️ Configuration

### Settings (in Options.xaml)
```xml
<CheckBox Content="Enable MCP (Equipment Control)" 
          IsChecked="{Binding MCPEnabled}" />
<TextBox Text="{Binding NinaHost}" />  <!-- Default: localhost -->
<TextBox Text="{Binding NinaPort}" />  <!-- Default: 1888 -->
```

### Programmatic Setup
```csharp
var mcpConfig = new MCPConfig
{
    Enabled = true,
    NinaHost = "localhost",
    NinaPort = 1888
};

// For Gemini
await googleProvider.EnableMCPAsync(mcpConfig);

// For Claude
await anthropicProvider.EnableMCPAsync(mcpConfig);
```

## 📊 Current Status

| Feature | Status | Notes |
|---------|--------|-------|
| Gemini MCP Support | ✅ Complete | Function calling API implemented |
| Claude MCP Support | ✅ Complete | Tool-use API existing |
| NINA Advanced API Client | ✅ Complete | 40+ tools defined |
| Multi-turn conversations | ✅ Complete | Both providers support iterations |
| Error handling | ✅ Complete | Enhanced logging and error reporting |
| External MCP server | ⏳ Optional | Can use nina_mcp_server separately |
| MCP server launcher | 📝 Future | Could integrate Python server startup |

## 🎓 System Prompts

### MCP-Enabled System Prompt (Both Providers)
```
You are an expert astrophotography assistant for N.I.N.A. with DIRECT CONTROL 
over imaging equipment through the NINA Advanced API.

IMPORTANT: You have FUNCTIONS/TOOLS that you MUST USE to interact with NINA. 
Do NOT just explain how to do things - USE THE TOOLS to actually do them.

When the user asks to check equipment, get status, or perform ANY action:
1. IMMEDIATELY use the appropriate function - do not just explain
2. Report the actual results from the function
3. Provide helpful interpretation of the data
```

## 🐛 Debugging

### Enable Debug Logging
Check NINA logs for MCP-prefixed entries:
```
[INFO] GoogleProvider: Using MCP function-calling flow
[INFO] [MCP] Executing function: nina_get_status
[DEBUG] [MCP] Function result: {"camera": {"connected": true, ...}}
[INFO] [MCP] Function nina_get_status completed - Success: True
```

### Common Issues
1. **"MCP connection failed"** → Check NINA Advanced API plugin is installed and running
2. **"Function not found"** → Check NINA Advanced API version (need v2.2.13+)
3. **"Maximum iterations reached"** → AI might be stuck in loop, check prompt clarity

## 📚 Related Documentation
- [NINA Advanced API](https://github.com/christian-photo/ninaAPI)
- [External MCP Server](https://github.com/michelebergo/nina_mcp_server)
- [MCP Investigation Doc](./MCP_INVESTIGATION.md)
- [Anthropic Tool Use](https://docs.anthropic.com/claude/docs/tool-use)
- [Gemini Function Calling](https://ai.google.dev/gemini-api/docs/function-calling)

## 🎯 Next Steps (Future Enhancements)

1. **External MCP Server Integration**
   - Add UI option to configure external MCP server path
   - Implement subprocess launcher for nina_advanced_mcp.py
   - Support switching between built-in and external MCP

2. **Additional Providers**
   - Explore MCP support for OpenAI (GPT-4 function calling)
   - Test with GitHub Models (via Azure OpenAI compatibility)

3. **Enhanced Tools**
   - Add more NINA Advanced API endpoints
   - Create composite tools for common workflows
   - Add image analysis tools

## ✨ Summary

**Key Achievement**: NINA AI Assistant now supports **MCP equipment control** for both **Google Gemini** and **Anthropic Claude**, enabling natural language control of telescopes, cameras, mounts, and all NINA equipment through AI conversations.

**Impact**: Users can now use either Gemini (free tier) or Claude (advanced reasoning) to control their astrophotography equipment via simple chat commands within NINA.

---
*Last Updated: January 21, 2026*  
*Plugin Version: 1.0.0.5+*  
*Contributors: Michele Bergo, GitHub Copilot*
