# Model Context Protocol (MCP) Investigation & Documentation

## Overview
The AI Assistant plugin implements Model Context Protocol (MCP) to allow AI models to directly control NINA equipment through the NINA Advanced API. This enables natural language equipment control like "slew to M31" or "start 300 second exposure at -10C".

## Current Implementation Status

### Architecture
```
User Message → Anthropic Claude (with tools) → MCP Tool Selection → NINA Advanced API → Equipment Action
                ↓                                                                         ↓
            Tool Results ←──────────────────────────────────────────────────────────────┘
                ↓
         Final Response to User
```

### Supported Features

#### ✅ Implemented
1. **Equipment Status & Connection**
   - `nina_get_status` - Get status of all equipment
   - `nina_get_version` - Get NINA version
   
2. **Camera Operations** (11 tools)
   - Connect/disconnect camera
   - Capture image with custom settings (exposure, gain, download, plate solve)
   - Start/stop cooling with target temperature
   - Abort exposure
   - Get camera info and capture statistics
   
3. **Mount Operations** (8 tools)
   - Connect/disconnect mount
   - Slew to coordinates (RA/Dec)
   - Park/unpark
   - Home mount
   - Stop slew
   - Set tracking mode (SIDEREAL, LUNAR, SOLAR, KING, STOPPED)
   
4. **Focuser Operations** (4 tools)
   - Connect/disconnect focuser
   - Move to absolute/relative position
   - Get focuser info
   
5. **Filter Wheel** (4 tools)
   - Connect/disconnect filter wheel
   - Change filter by ID
   - Get filter wheel info
   
6. **Guider** (6 tools)
   - Connect/disconnect guider (PHD2)
   - Start/stop guiding
   - Calibrate guider
   - Get guider info
   
7. **Dome** (4+ tools)
   - Connect/disconnect dome
   - Open/close shutter
   - Get dome info

### How It Works

#### 1. MCP Configuration (`MCPConfig`)
```csharp
public class MCPConfig
{
    public bool Enabled { get; set; } = true;
    public string NinaHost { get; set; } = "localhost";
    public int NinaPort { get; set; } = 1888;  // NINA Advanced API port
    public string ImageSaveDir { get; set; } = "";
}
```

#### 2. Tool Definition
Each tool follows Anthropic's tool format:
```json
{
  "name": "nina_capture_image",
  "description": "Capture an image with the camera",
  "input_schema": {
    "type": "object",
    "properties": {
      "duration": { "type": "number", "description": "Exposure time in seconds" },
      "gain": { "type": "integer", "description": "Camera gain setting" }
    }
  }
}
```

#### 3. Tool Execution Flow (AnthropicProvider)
```csharp
// 1. Send request with tools available
POST https://api.anthropic.com/v1/messages
{
  "model": "claude-3-7-sonnet-20250219",
  "tools": [...],  // All 40+ NINA tools
  "messages": [...]
}

// 2. Claude decides to use tool
Response: {
  "stop_reason": "tool_use",
  "content": [
    { "type": "tool_use", "name": "nina_capture_image", "input": {...} }
  ]
}

// 3. Execute tool via NINA Advanced API
GET http://localhost:1888/v2/api/equipment/camera/capture?duration=60&gain=100

// 4. Return tool result to Claude
POST /messages again with tool_result

// 5. Claude generates final response
"Image captured successfully with 300 second exposure at gain 100"
```

#### 4. NINA Advanced API Integration
The `NINAAdvancedAPIClient` directly calls NINA's HTTP API:

```csharp
// Example: Capture Image
public async Task<MCPToolResult> InvokeToolAsync(string toolName, Dictionary<string, object>? args)
{
    if (toolName == "nina_capture_image")
    {
        var duration = args?["duration"] as double? ?? 1.0;
        var gain = args?["gain"] as int? ?? 100;
        
        var url = $"http://{_config.NinaHost}:{_config.NinaPort}/v2/api/equipment/camera/capture";
        var body = new { duration, gain, download = true };
        
        var response = await _httpClient.PostAsync(url, JsonContent(body));
        // ... handle response
    }
}
```

### Key Implementation Details

#### Multi-Turn Tool Calling
The implementation supports multiple tool calls in sequence:
```csharp
private const int MaxToolIterations = 10; // Prevent infinite loops

while (iterations < MaxToolIterations)
{
    // 1. Send request with tools
    // 2. Execute any tool_use responses
    // 3. Send tool results back
    // 4. Repeat if more tools needed
    // 5. Return when stop_reason != "tool_use"
}
```

#### Tool Safety
- **Iteration Limit**: Maximum 10 tool calls per conversation to prevent runaway loops
- **Error Handling**: Failed tools return descriptive error messages to Claude
- **Connection Check**: MCP only enabled if Advanced API is reachable

#### Provider Support
- **✅ Anthropic Claude**: Full MCP support (native tool calling API)
- **⚠️ OpenAI**: Could be added (function calling API)
- **⚠️ Google Gemini**: Could be added (function calling API)
- **❌ GitHub Models**: Limited (depends on model support)
- **❌ Ollama**: Depends on model
- **❌ OpenRouter**: Depends on selected model

## Testing & Investigation

### How to Test MCP Functionality

#### Prerequisites
1. NINA must be running
2. Advanced API must be enabled:
   - Options → Advanced API → Enable
   - Default port: 1888
3. Equipment connected in NINA (camera, mount, etc.)

#### Test Commands
```
User: "What is my camera temperature?"
Expected: Tool call to nina_get_camera_info, return current temperature

User: "Take a 60 second exposure at gain 100"
Expected: Tool call to nina_capture_image with duration=60, gain=100

User: "Slew to M31"
Expected: Multiple tools - get object coordinates, then nina_slew_mount

User: "Start cooling to -10C"
Expected: Tool call to nina_start_cooling with temperature=-10
```

### Logging Investigation Points

#### Current Logging
The plugin logs MCP activity:
```
[INFO] AnthropicProvider: Using MCP tool-calling flow
[INFO] AnthropicProvider: Sending request with 40 MCP tools available
[INFO] AnthropicProvider: Tool iteration 1
[INFO] Executing MCP tool: nina_capture_image
[INFO] MCP tool nina_capture_image executed successfully
```

#### Add More Detailed Logging
To investigate issues, add logging at these points:

1. **Tool Selection**
```csharp
Logger.Debug($"Claude selected tool: {toolName} with args: {JsonConvert.SerializeObject(toolInput)}");
```

2. **API Requests**
```csharp
Logger.Debug($"Calling NINA API: {url}");
Logger.Debug($"Request body: {json}");
```

3. **API Responses**
```csharp
Logger.Debug($"NINA API response ({response.StatusCode}): {responseContent}");
```

4. **Tool Results**
```csharp
Logger.Debug($"Tool result - Success: {result.Success}, Content: {result.Content}");
```

### Known Limitations

1. **No Streaming**: Tool calls are synchronous, blocking until complete
2. **No Parallel Tools**: Tools execute sequentially
3. **Limited Error Recovery**: If tool fails, Claude may not retry
4. **No File Upload**: Cannot pass image data to Claude for analysis
5. **Fixed Port**: Currently hardcoded to 1888, should be configurable

### Enhancement Opportunities

#### High Priority
1. **Add OpenAI Function Calling**
   - OpenAI has similar tool/function calling API
   - Would enable MCP for GPT-4o users
   
2. **Enhanced Error Messages**
   - Return structured error data to help Claude understand failures
   - Include suggestions for corrective action
   
3. **Tool Result Formatting**
   - Better formatting of equipment status for Claude
   - Include relevant metadata (e.g., "Camera cooled to target in 5 minutes")

#### Medium Priority
4. **Google Gemini Function Calling**
   - Add support for Gemini's function calling
   - Test with gemini-2.0-flash-exp
   
5. **Tool Descriptions Improvements**
   - Add more context about when to use each tool
   - Include example values in descriptions
   - Add hints about prerequisites (e.g., "camera must be connected")
   
6. **Configuration UI**
   - Add UI for enabling/disabling specific tool categories
   - Port configuration in settings panel

#### Low Priority
7. **Tool Usage Analytics**
   - Track which tools are used most
   - Log success/failure rates
   - Help users understand what works

8. **Safety Guards**
   - Require confirmation for destructive actions (e.g., "park mount")
   - Add safety limits (max slew distance, max exposure time)
   - Emergency stop functionality

## NINA Advanced API Endpoints

### Available Endpoints (v2 API)
```
GET  /v2/api/version                      - Get NINA version
GET  /v2/api/equipment/status             - All equipment status

# Camera
POST /v2/api/equipment/camera/connect
POST /v2/api/equipment/camera/disconnect
GET  /v2/api/equipment/camera/info
POST /v2/api/equipment/camera/capture
POST /v2/api/equipment/camera/cooling/start
POST /v2/api/equipment/camera/cooling/stop
POST /v2/api/equipment/camera/abort

# Mount
POST /v2/api/equipment/mount/connect
POST /v2/api/equipment/mount/disconnect
GET  /v2/api/equipment/mount/info
POST /v2/api/equipment/mount/slew
POST /v2/api/equipment/mount/park
POST /v2/api/equipment/mount/unpark
POST /v2/api/equipment/mount/home
POST /v2/api/equipment/mount/tracking

# Focuser
POST /v2/api/equipment/focuser/connect
POST /v2/api/equipment/focuser/disconnect
GET  /v2/api/equipment/focuser/info
POST /v2/api/equipment/focuser/move

# Filter Wheel
POST /v2/api/equipment/filterwheel/connect
POST /v2/api/equipment/filterwheel/disconnect
GET  /v2/api/equipment/filterwheel/info
POST /v2/api/equipment/filterwheel/changefilter

# Guider
POST /v2/api/equipment/guider/connect
POST /v2/api/equipment/guider/disconnect
GET  /v2/api/equipment/guider/info
POST /v2/api/equipment/guider/start
POST /v2/api/equipment/guider/stop

# Dome
POST /v2/api/equipment/dome/connect
POST /v2/api/equipment/dome/disconnect
GET  /v2/api/equipment/dome/info
POST /v2/api/equipment/dome/shutter/open
POST /v2/api/equipment/dome/shutter/close
```

### Testing NINA API Directly
You can test the Advanced API using curl or Postman:

```powershell
# Get NINA version
curl http://localhost:1888/v2/api/version

# Get all equipment status
curl http://localhost:1888/v2/api/equipment/status

# Get camera info
curl http://localhost:1888/v2/api/equipment/camera/info

# Capture image (POST with JSON body)
curl -X POST http://localhost:1888/v2/api/equipment/camera/capture -H "Content-Type: application/json" -d "{\"duration\": 1, \"gain\": 100}"
```

## Summary

The MCP implementation is **functional** but has room for enhancement:

✅ **Working**: Anthropic Claude can control NINA equipment via 40+ tools  
⚠️ **Limited**: Only Anthropic provider has MCP support  
🔧 **Needs Work**: Error handling, logging, configuration UI  
🚀 **Potential**: Could add OpenAI/Google function calling support  

The foundation is solid - the tool definitions are comprehensive and the API integration is complete. The main gaps are in provider coverage and user experience.
