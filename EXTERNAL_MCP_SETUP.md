# External MCP Server Setup Guide

## Overview

The AI Assistant plugin now supports **both** built-in NINA Advanced API tools AND external MCP servers. This allows you to extend the plugin with custom tools written in Python.

## Architecture

```
User → AI Assistant Plugin (Gemini/Claude)
    ├──> Built-in: NINA Advanced API (100+ tools) [Direct HTTP]
    └──> External: Custom MCP Server [stdio/subprocess]
```

## Setup External MCP Server

### 1. Install Python Dependencies

```bash
cd "C:\Users\miche\Desktop\NINA_AI_plugin\NINA.Plugin.AIAssistant\MCP"
pip install -r requirements.txt
```

**Required packages:**
- `mcp>=1.1.0` - Model Context Protocol SDK
- `httpx>=0.27.0` - Async HTTP client

### 2. Test the NINA Advanced API MCP Server

```bash
# Test the server directly
python nina_advanced_api_mcp_server.py
```

The server should start and wait for JSON-RPC messages via stdin/stdout.

### 3. Configure Plugin Settings

1. Open NINA
2. Go to **Options → Plugins → AI Assistant**
3. Configure External MCP settings:
   - **External MCP Python Path**: `python` (or full path like `C:\Python312\python.exe`)
   - **External MCP Script Path**: `C:\Users\miche\Desktop\NINA_AI_plugin\NINA.Plugin.AIAssistant\MCP\nina_advanced_api_mcp_server.py`
4. Ensure **MCP Enabled** is checked
5. Save settings

### 4. Verify Connection

1. Restart NINA
2. Open AI Assistant
3. Check for status message: **"🤖 MCP Connected (Gemini)"**
4. Look in NINA logs for:
   ```
   [MCP] External MCP server started: nina-advanced-api-server v1.0.0
   GoogleProvider: Added X built-in NINA API tools
   GoogleProvider: Added Y external MCP tools
   ```

## How It Works

### Tool Discovery

When the plugin starts, it:
1. Connects to built-in NINA Advanced API (100+ tools)
2. Launches external Python MCP server as subprocess
3. Queries external server for available tools
4. **Merges** both tool sets and sends to AI

### Tool Execution

When AI wants to call a tool:
1. Plugin tries built-in API first
2. If tool not found, routes to external MCP server
3. Returns result to AI for next step

### Example Flow

```
User: "What version of NINA am I running?"
  → AI decides to use: nina_get_version
  → Plugin checks: Built-in API has this tool
  → Executes: GET http://localhost:1888/v2/api/version
  → Returns: "2.2.14.1"
  
User: "Calculate the optimal imaging time for M31"
  → AI decides to use: nina_calculate_optimal_time (custom tool)
  → Plugin checks: Not in built-in API
  → Routes to: External MCP server
  → Executes: Python custom logic
  → Returns: Calculated result
```

## Extending with Custom Tools

### Modify the Python Server

Edit `nina_advanced_api_mcp_server.py` and add your custom tools:

```python
@server.list_tools()
async def handle_list_tools() -> list[Tool]:
    tools = [
        # ... existing tools ...
        
        # Add your custom tool
        Tool(
            name="nina_calculate_moon_phase",
            description="Calculate moon phase for a given date",
            inputSchema={
                "type": "object",
                "properties": {
                    "date": {"type": "string", "description": "Date in ISO format"}
                },
                "required": ["date"]
            }
        ),
    ]
    return tools

@server.call_tool()
async def handle_call_tool(name: str, arguments: dict) -> list[TextContent]:
    if name == "nina_calculate_moon_phase":
        # Your custom logic here
        from datetime import datetime
        import ephem
        
        date = arguments.get('date')
        moon = ephem.Moon(date)
        phase = moon.phase
        
        return [TextContent(
            type="text",
            text=f"Moon phase on {date}: {phase:.1f}%"
        )]
    
    # ... existing tool handling ...
```

### Example Custom Tools

- **Astronomy calculations**: Rise/set times, optimal imaging windows
- **Weather integration**: Clear Outside, weather forecast APIs
- **Database queries**: Target catalog lookups, observation logs
- **File operations**: Manage image archives, sequence files
- **Third-party APIs**: Telescope.live, Astrobin integration
- **Custom hardware**: Arduino sensors, custom focusers

## Debugging

### Enable Verbose Logging

Check NINA logs at: `%LOCALAPPDATA%\NINA\Logs\`

Look for:
```
[MCP] Starting external MCP server: ...
[MCP] Connected to nina-advanced-api-server v1.0.0
GoogleProvider: Added 100 built-in NINA API tools
GoogleProvider: Added 100 external MCP tools
[MCP] Executing function: nina_get_version
[MCP] Used external MCP server for nina_custom_tool
```

### Test Server Standalone

```bash
# Test stdio communication
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2024-11-05"}}' | python nina_advanced_api_mcp_server.py
```

### Common Issues

**Server not starting:**
- Check Python path is correct
- Verify `mcp` and `httpx` packages installed
- Check script path is absolute

**Tools not showing up:**
- Verify server started (check logs)
- Ensure external tools have unique names (don't conflict with built-in)
- Restart NINA after changing server code

**Tool execution fails:**
- Check NINA Advanced API is running (localhost:1888)
- Verify endpoint mappings in `map_tool_to_endpoint()`
- Check function arguments match tool schema

## Performance Notes

- **Tool Count**: 100+ built-in + custom = ~200 total tools
- **Startup**: External server adds ~1-2 seconds to initialization
- **Execution**: External tools slightly slower than built-in (subprocess overhead)
- **Memory**: Python process ~30-50MB

## Current Status

✅ **Built-in NINA API**: 100+ tools (Camera, Mount, Focuser, Rotator, Flat Panel, Switch, Weather, Safety, Sequences, Plate Solving, Framing, Utility)
✅ **External MCP Framework**: Complete, tested with Gemini
⏳ **Claude Support**: External MCP coming soon (currently only built-in)
🚧 **Custom Tools**: Template ready, add your own!

## Next Steps

1. **Test the setup** with provided NINA Advanced API MCP server
2. **Verify** both built-in and external tools work
3. **Add custom tools** for your specific needs
4. **Share** useful custom tools with the community!
