#!/usr/bin/env python3
"""
NINA Advanced API MCP Server
Exposes NINA Advanced API endpoints as MCP tools via stdio transport
"""

import asyncio
import httpx
import logging
from typing import Any, Optional
from mcp.server.models import InitializationOptions
from mcp.server import NotificationOptions, Server
from mcp.server.stdio import stdio_server
from mcp.types import (
    Tool,
    TextContent,
    INVALID_PARAMS,
    INTERNAL_ERROR
)

# Configure logging to stderr (stdout is reserved for MCP protocol)
logging.basicConfig(
    level=logging.INFO,
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    handlers=[logging.StreamHandler()]
)
logger = logging.getLogger("nina-mcp-server")

# NINA Advanced API base URL
BASE_URL = "http://localhost:1888/v2/api"

# Create MCP server instance
server = Server("nina-advanced-api-server")

# HTTP client for API calls
http_client = httpx.AsyncClient(timeout=30.0)


@server.list_tools()
async def handle_list_tools() -> list[Tool]:
    """List all available NINA Advanced API tools"""
    
    tools = [
        # System
        Tool(
            name="nina_get_version",
            description="Get NINA application version",
            inputSchema={
                "type": "object",
                "properties": {},
                "required": []
            }
        ),
        
        # Camera
        Tool(
            name="nina_connect_camera",
            description="Connect to a camera device",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_camera",
            description="Disconnect the camera",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_camera_info",
            description="Get camera information and status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_cameras",
            description="List all available cameras",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_capture_image",
            description="Capture an image with the camera",
            inputSchema={
                "type": "object",
                "properties": {
                    "exposure_time": {"type": "number", "description": "Exposure time in seconds"},
                    "binning": {"type": "integer", "description": "Binning factor (1, 2, 3, 4)", "default": 1},
                    "gain": {"type": "integer", "description": "Gain value", "default": 0}
                },
                "required": ["exposure_time"]
            }
        ),
        Tool(
            name="nina_start_cooling",
            description="Start camera cooling to target temperature",
            inputSchema={
                "type": "object",
                "properties": {
                    "temperature": {"type": "number", "description": "Target temperature in Celsius"},
                    "duration": {"type": "integer", "description": "Duration in minutes", "default": 0}
                },
                "required": ["temperature"]
            }
        ),
        Tool(
            name="nina_stop_cooling",
            description="Stop camera cooling",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_set_binning",
            description="Set camera binning",
            inputSchema={
                "type": "object",
                "properties": {
                    "binning": {"type": "integer", "description": "Binning factor (1, 2, 3, 4)"}
                },
                "required": ["binning"]
            }
        ),
        Tool(
            name="nina_control_dew_heater",
            description="Control camera dew heater",
            inputSchema={
                "type": "object",
                "properties": {
                    "on": {"type": "boolean", "description": "Turn heater on (true) or off (false)"}
                },
                "required": ["on"]
            }
        ),
        Tool(
            name="nina_set_gain",
            description="Set camera gain",
            inputSchema={
                "type": "object",
                "properties": {
                    "gain": {"type": "integer", "description": "Gain value"}
                },
                "required": ["gain"]
            }
        ),
        Tool(
            name="nina_set_offset",
            description="Set camera offset",
            inputSchema={
                "type": "object",
                "properties": {
                    "offset": {"type": "integer", "description": "Offset value"}
                },
                "required": ["offset"]
            }
        ),
        Tool(
            name="nina_start_warming",
            description="Start warming camera to ambient temperature",
            inputSchema={
                "type": "object",
                "properties": {
                    "duration": {"type": "integer", "description": "Warming duration in minutes"}
                },
                "required": ["duration"]
            }
        ),
        
        # Mount (Telescope)
        Tool(
            name="nina_connect_telescope",
            description="Connect to telescope mount",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_telescope",
            description="Disconnect telescope mount",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_telescope_info",
            description="Get telescope mount information and coordinates",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_telescopes",
            description="List all available telescope mounts",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_slew_telescope",
            description="Slew telescope to coordinates",
            inputSchema={
                "type": "object",
                "properties": {
                    "ra": {"type": "number", "description": "Right Ascension in hours (0-24)"},
                    "dec": {"type": "number", "description": "Declination in degrees (-90 to +90)"}
                },
                "required": ["ra", "dec"]
            }
        ),
        Tool(
            name="nina_park_telescope",
            description="Park the telescope mount",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_unpark_telescope",
            description="Unpark the telescope mount",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_stop_telescope",
            description="Stop telescope movement immediately",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Focuser
        Tool(
            name="nina_connect_focuser",
            description="Connect to focuser",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_focuser",
            description="Disconnect focuser",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_focuser_info",
            description="Get focuser position and status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_focusers",
            description="List all available focusers",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_move_focuser",
            description="Move focuser to absolute position",
            inputSchema={
                "type": "object",
                "properties": {
                    "position": {"type": "integer", "description": "Target position"}
                },
                "required": ["position"]
            }
        ),
        Tool(
            name="nina_start_autofocus",
            description="Start autofocus routine",
            inputSchema={
                "type": "object",
                "properties": {
                    "method": {"type": "string", "description": "Autofocus method", "default": ""}
                },
                "required": []
            }
        ),
        Tool(
            name="nina_cancel_autofocus",
            description="Cancel running autofocus",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_autofocus_status",
            description="Get autofocus routine status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_halt_focuser",
            description="Halt focuser movement immediately",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Filter Wheel
        Tool(
            name="nina_connect_filterwheel",
            description="Connect to filter wheel",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_filterwheel",
            description="Disconnect filter wheel",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_filterwheel_info",
            description="Get filter wheel information and current filter",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_filterwheels",
            description="List all available filter wheels",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_change_filter",
            description="Change to specified filter",
            inputSchema={
                "type": "object",
                "properties": {
                    "filter": {"type": "string", "description": "Filter name"}
                },
                "required": ["filter"]
            }
        ),
        
        # Rotator
        Tool(
            name="nina_connect_rotator",
            description="Connect to rotator",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_rotator",
            description="Disconnect rotator",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_rotators",
            description="List all available rotators",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_rotator_info",
            description="Get rotator angle and status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_move_rotator",
            description="Move rotator to angle",
            inputSchema={
                "type": "object",
                "properties": {
                    "position": {"type": "number", "description": "Target angle in degrees"},
                    "relative": {"type": "boolean", "description": "Relative movement", "default": False}
                },
                "required": ["position"]
            }
        ),
        Tool(
            name="nina_halt_rotator",
            description="Halt rotator movement",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_sync_rotator",
            description="Sync rotator to mechanical position",
            inputSchema={
                "type": "object",
                "properties": {
                    "position": {"type": "number", "description": "Mechanical position"}
                },
                "required": ["position"]
            }
        ),
        Tool(
            name="nina_set_rotator_reverse",
            description="Set rotator reverse direction",
            inputSchema={
                "type": "object",
                "properties": {
                    "reverse": {"type": "boolean", "description": "Reverse direction"}
                },
                "required": ["reverse"]
            }
        ),
        
        # Flat Panel
        Tool(
            name="nina_connect_flatpanel",
            description="Connect to flat panel",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_flatpanel",
            description="Disconnect flat panel",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_flatpanels",
            description="List all available flat panels",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_flatpanel_info",
            description="Get flat panel status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_set_flatpanel_light",
            description="Control flat panel light",
            inputSchema={
                "type": "object",
                "properties": {
                    "power": {"type": "boolean", "description": "Turn light on (true) or off (false)"}
                },
                "required": ["power"]
            }
        ),
        Tool(
            name="nina_set_flatpanel_cover",
            description="Control flat panel cover",
            inputSchema={
                "type": "object",
                "properties": {
                    "open": {"type": "boolean", "description": "Open cover (true) or close (false)"}
                },
                "required": ["open"]
            }
        ),
        Tool(
            name="nina_set_flatpanel_brightness",
            description="Set flat panel brightness",
            inputSchema={
                "type": "object",
                "properties": {
                    "brightness": {"type": "integer", "description": "Brightness value (0-100)"}
                },
                "required": ["brightness"]
            }
        ),
        
        # Switch
        Tool(
            name="nina_connect_switch",
            description="Connect to switch device",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_switch",
            description="Disconnect switch device",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_switches",
            description="List all available switches",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_switch_channels",
            description="Get available switch channels",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_set_switch",
            description="Set switch channel value",
            inputSchema={
                "type": "object",
                "properties": {
                    "index": {"type": "integer", "description": "Channel index"},
                    "value": {"type": "number", "description": "Value to set"}
                },
                "required": ["index", "value"]
            }
        ),
        
        # Weather
        Tool(
            name="nina_connect_weather",
            description="Connect to weather station",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_weather",
            description="Disconnect weather station",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_weather_info",
            description="Get current weather data",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_weather_sources",
            description="List all available weather sources",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Safety Monitor
        Tool(
            name="nina_connect_safetymonitor",
            description="Connect to safety monitor",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_safetymonitor",
            description="Disconnect safety monitor",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_safetymonitor_info",
            description="Get safety monitor status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_safetymonitors",
            description="List all available safety monitors",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Guider
        Tool(
            name="nina_connect_guider",
            description="Connect to guider",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_guider",
            description="Disconnect guider",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_guider_info",
            description="Get guider status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_guiders",
            description="List all available guiders",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_start_guiding",
            description="Start guiding",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_stop_guiding",
            description="Stop guiding",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_dither",
            description="Dither the guider",
            inputSchema={
                "type": "object",
                "properties": {
                    "pixels": {"type": "number", "description": "Dither amount in pixels"}
                },
                "required": ["pixels"]
            }
        ),
        
        # Dome
        Tool(
            name="nina_connect_dome",
            description="Connect to dome",
            inputSchema={
                "type": "object",
                "properties": {
                    "device_id": {"type": "string", "description": "Device ID to connect to"}
                },
                "required": ["device_id"]
            }
        ),
        Tool(
            name="nina_disconnect_dome",
            description="Disconnect dome",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_get_dome_info",
            description="Get dome status",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_list_domes",
            description="List all available domes",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_open_dome_shutter",
            description="Open dome shutter",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_close_dome_shutter",
            description="Close dome shutter",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_slew_dome",
            description="Slew dome to azimuth",
            inputSchema={
                "type": "object",
                "properties": {
                    "azimuth": {"type": "number", "description": "Azimuth in degrees (0-360)"}
                },
                "required": ["azimuth"]
            }
        ),
        
        # Sequences
        Tool(
            name="nina_sequence_start",
            description="Start the current sequence",
            inputSchema={
                "type": "object",
                "properties": {
                    "skipValidation": {"type": "boolean", "description": "Skip validation", "default": False}
                },
                "required": []
            }
        ),
        Tool(
            name="nina_sequence_stop",
            description="Stop the running sequence",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_sequence_load",
            description="Load a sequence from file",
            inputSchema={
                "type": "object",
                "properties": {
                    "filepath": {"type": "string", "description": "Path to sequence file"}
                },
                "required": ["filepath"]
            }
        ),
        Tool(
            name="nina_sequence_json",
            description="Get sequence as JSON",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Plate Solving
        Tool(
            name="nina_platesolve_capsolve",
            description="Capture image and solve plate",
            inputSchema={
                "type": "object",
                "properties": {
                    "blind": {"type": "boolean", "description": "Use blind solve", "default": False}
                },
                "required": []
            }
        ),
        Tool(
            name="nina_platesolve_sync",
            description="Plate solve and sync mount",
            inputSchema={
                "type": "object",
                "properties": {
                    "blind": {"type": "boolean", "description": "Use blind solve", "default": False}
                },
                "required": []
            }
        ),
        Tool(
            name="nina_platesolve_center",
            description="Center on coordinates using plate solving",
            inputSchema={
                "type": "object",
                "properties": {
                    "ra": {"type": "number", "description": "Right Ascension in hours"},
                    "dec": {"type": "number", "description": "Declination in degrees"}
                },
                "required": ["ra", "dec"]
            }
        ),
        
        # Framing Assistant
        Tool(
            name="nina_framing_get_info",
            description="Get framing assistant information",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_framing_set_source",
            description="Set framing target source",
            inputSchema={
                "type": "object",
                "properties": {
                    "source": {"type": "string", "description": "Target name or coordinates"}
                },
                "required": ["source"]
            }
        ),
        Tool(
            name="nina_framing_slew",
            description="Slew to framing target",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        
        # Utility
        Tool(
            name="nina_time_now",
            description="Get current system time",
            inputSchema={"type": "object", "properties": {}, "required": []}
        ),
        Tool(
            name="nina_wait",
            description="Wait for specified duration",
            inputSchema={
                "type": "object",
                "properties": {
                    "seconds": {"type": "integer", "description": "Seconds to wait"}
                },
                "required": ["seconds"]
            }
        ),
    ]
    
    logger.info(f"Listing {len(tools)} tools")
    return tools


@server.call_tool()
async def handle_call_tool(name: str, arguments: dict) -> list[TextContent]:
    """Execute NINA Advanced API tool"""
    
    try:
        logger.info(f"Calling tool: {name} with args: {arguments}")
        
        # Map tool name to API endpoint
        endpoint = map_tool_to_endpoint(name, arguments)
        
        if not endpoint:
            return [TextContent(
                type="text",
                text=f"Error: Unknown tool {name}"
            )]
        
        # Make API call
        url = f"{BASE_URL}/{endpoint}"
        logger.info(f"API call: {url}")
        
        response = await http_client.get(url)
        response.raise_for_status()
        
        result = response.json()
        
        return [TextContent(
            type="text",
            text=str(result)
        )]
        
    except Exception as e:
        logger.error(f"Tool execution failed: {str(e)}")
        return [TextContent(
            type="text",
            text=f"Error: {str(e)}"
        )]


def map_tool_to_endpoint(tool_name: str, args: dict) -> Optional[str]:
    """Map tool name to NINA Advanced API endpoint"""
    
    # System
    if tool_name == "nina_get_version":
        return "version"
    
    # Camera
    elif tool_name == "nina_connect_camera":
        return f"equipment/camera/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_camera":
        return "equipment/camera/disconnect"
    elif tool_name == "nina_get_camera_info":
        return "equipment/camera/info"
    elif tool_name == "nina_list_cameras":
        return "equipment/camera/list"
    elif tool_name == "nina_capture_image":
        exp = args.get('exposure_time', 1)
        binning = args.get('binning', 1)
        gain = args.get('gain', 0)
        return f"equipment/camera/capture?exposuretime={exp}&binning={binning}&gain={gain}"
    elif tool_name == "nina_start_cooling":
        temp = args.get('temperature', -10)
        duration = args.get('duration', 0)
        return f"equipment/camera/cooling?temperature={temp}&duration={duration}"
    elif tool_name == "nina_stop_cooling":
        return "equipment/camera/warmup"
    elif tool_name == "nina_set_binning":
        binning = args.get('binning', 1)
        return f"equipment/camera/binning?x={binning}&y={binning}"
    elif tool_name == "nina_control_dew_heater":
        on = str(args.get('on', False)).lower()
        return f"equipment/camera/dew-heater?on={on}"
    elif tool_name == "nina_set_gain":
        gain = args.get('gain', 0)
        return f"equipment/camera/gain?gain={gain}"
    elif tool_name == "nina_set_offset":
        offset = args.get('offset', 0)
        return f"equipment/camera/offset?offset={offset}"
    elif tool_name == "nina_start_warming":
        duration = args.get('duration', 10)
        return f"equipment/camera/warmup?duration={duration}"
    
    # Mount
    elif tool_name == "nina_connect_telescope":
        return f"equipment/telescope/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_telescope":
        return "equipment/telescope/disconnect"
    elif tool_name == "nina_get_telescope_info":
        return "equipment/telescope/info"
    elif tool_name == "nina_list_telescopes":
        return "equipment/telescope/list"
    elif tool_name == "nina_slew_telescope":
        ra = args.get('ra', 0)
        dec = args.get('dec', 0)
        return f"equipment/telescope/slew?rightascension={ra}&declination={dec}"
    elif tool_name == "nina_park_telescope":
        return "equipment/telescope/park"
    elif tool_name == "nina_unpark_telescope":
        return "equipment/telescope/unpark"
    elif tool_name == "nina_stop_telescope":
        return "equipment/telescope/abort-slew"
    
    # Focuser
    elif tool_name == "nina_connect_focuser":
        return f"equipment/focuser/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_focuser":
        return "equipment/focuser/disconnect"
    elif tool_name == "nina_get_focuser_info":
        return "equipment/focuser/info"
    elif tool_name == "nina_list_focusers":
        return "equipment/focuser/list"
    elif tool_name == "nina_move_focuser":
        pos = args.get('position', 0)
        return f"equipment/focuser/move?position={pos}"
    elif tool_name == "nina_start_autofocus":
        method = args.get('method', '')
        if method:
            return f"equipment/focuser/autofocus?method={method}"
        return "equipment/focuser/autofocus"
    elif tool_name == "nina_cancel_autofocus":
        return "equipment/focuser/autofocus-cancel"
    elif tool_name == "nina_get_autofocus_status":
        return "equipment/focuser/autofocus-status"
    elif tool_name == "nina_halt_focuser":
        return "equipment/focuser/halt"
    
    # Filter Wheel
    elif tool_name == "nina_connect_filterwheel":
        return f"equipment/filterwheel/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_filterwheel":
        return "equipment/filterwheel/disconnect"
    elif tool_name == "nina_get_filterwheel_info":
        return "equipment/filterwheel/info"
    elif tool_name == "nina_list_filterwheels":
        return "equipment/filterwheel/list"
    elif tool_name == "nina_change_filter":
        filter_name = args.get('filter', '')
        return f"equipment/filterwheel/set-filter?filter={filter_name}"
    
    # Rotator
    elif tool_name == "nina_connect_rotator":
        return f"equipment/rotator/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_rotator":
        return "equipment/rotator/disconnect"
    elif tool_name == "nina_list_rotators":
        return "equipment/rotator/list"
    elif tool_name == "nina_get_rotator_info":
        return "equipment/rotator/info"
    elif tool_name == "nina_move_rotator":
        pos = args.get('position', 0)
        rel = str(args.get('relative', False)).lower()
        return f"equipment/rotator/move?position={pos}&relative={rel}"
    elif tool_name == "nina_halt_rotator":
        return "equipment/rotator/halt"
    elif tool_name == "nina_sync_rotator":
        pos = args.get('position', 0)
        return f"equipment/rotator/sync?mechanicalposition={pos}"
    elif tool_name == "nina_set_rotator_reverse":
        reverse = str(args.get('reverse', False)).lower()
        return f"equipment/rotator/reverse?reverse={reverse}"
    
    # Flat Panel
    elif tool_name == "nina_connect_flatpanel":
        return f"equipment/flatdevice/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_flatpanel":
        return "equipment/flatdevice/disconnect"
    elif tool_name == "nina_list_flatpanels":
        return "equipment/flatdevice/list"
    elif tool_name == "nina_get_flatpanel_info":
        return "equipment/flatdevice/info"
    elif tool_name == "nina_set_flatpanel_light":
        power = str(args.get('power', False)).lower()
        return f"equipment/flatdevice/set-light?power={power}"
    elif tool_name == "nina_set_flatpanel_cover":
        open_cover = str(args.get('open', False)).lower()
        return f"equipment/flatdevice/set-cover?open={open_cover}"
    elif tool_name == "nina_set_flatpanel_brightness":
        brightness = args.get('brightness', 50)
        return f"equipment/flatdevice/set-brightness?brightness={brightness}"
    
    # Switch
    elif tool_name == "nina_connect_switch":
        return f"equipment/switch/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_switch":
        return "equipment/switch/disconnect"
    elif tool_name == "nina_list_switches":
        return "equipment/switch/list"
    elif tool_name == "nina_get_switch_channels":
        return "equipment/switch/channels"
    elif tool_name == "nina_set_switch":
        index = args.get('index', 0)
        value = args.get('value', 0)
        return f"equipment/switch/set?index={index}&value={value}"
    
    # Weather
    elif tool_name == "nina_connect_weather":
        return f"equipment/weather/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_weather":
        return "equipment/weather/disconnect"
    elif tool_name == "nina_get_weather_info":
        return "equipment/weather/info"
    elif tool_name == "nina_list_weather_sources":
        return "equipment/weather/list"
    
    # Safety Monitor
    elif tool_name == "nina_connect_safetymonitor":
        return f"equipment/safetymonitor/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_safetymonitor":
        return "equipment/safetymonitor/disconnect"
    elif tool_name == "nina_get_safetymonitor_info":
        return "equipment/safetymonitor/info"
    elif tool_name == "nina_list_safetymonitors":
        return "equipment/safetymonitor/list"
    
    # Guider
    elif tool_name == "nina_connect_guider":
        return f"equipment/guider/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_guider":
        return "equipment/guider/disconnect"
    elif tool_name == "nina_get_guider_info":
        return "equipment/guider/info"
    elif tool_name == "nina_list_guiders":
        return "equipment/guider/list"
    elif tool_name == "nina_start_guiding":
        return "equipment/guider/start-guiding"
    elif tool_name == "nina_stop_guiding":
        return "equipment/guider/stop-guiding"
    elif tool_name == "nina_dither":
        pixels = args.get('pixels', 5)
        return f"equipment/guider/dither?pixels={pixels}"
    
    # Dome
    elif tool_name == "nina_connect_dome":
        return f"equipment/dome/connect?to={args.get('device_id', '')}"
    elif tool_name == "nina_disconnect_dome":
        return "equipment/dome/disconnect"
    elif tool_name == "nina_get_dome_info":
        return "equipment/dome/info"
    elif tool_name == "nina_list_domes":
        return "equipment/dome/list"
    elif tool_name == "nina_open_dome_shutter":
        return "equipment/dome/open-shutter"
    elif tool_name == "nina_close_dome_shutter":
        return "equipment/dome/close-shutter"
    elif tool_name == "nina_slew_dome":
        azimuth = args.get('azimuth', 0)
        return f"equipment/dome/slew?azimuth={azimuth}"
    
    # Sequences
    elif tool_name == "nina_sequence_start":
        skip = str(args.get('skipValidation', False)).lower()
        return f"sequence/start?skipValidation={skip}"
    elif tool_name == "nina_sequence_stop":
        return "sequence/stop"
    elif tool_name == "nina_sequence_load":
        filepath = args.get('filepath', '')
        return f"sequence/load?filepath={filepath}"
    elif tool_name == "nina_sequence_json":
        return "sequence/json"
    
    # Plate Solving
    elif tool_name == "nina_platesolve_capsolve":
        blind = str(args.get('blind', False)).lower()
        return f"plate-solve/capsolve?blind={blind}"
    elif tool_name == "nina_platesolve_sync":
        blind = str(args.get('blind', False)).lower()
        return f"plate-solve/sync?blind={blind}"
    elif tool_name == "nina_platesolve_center":
        ra = args.get('ra', 0)
        dec = args.get('dec', 0)
        return f"plate-solve/center?rightascension={ra}&declination={dec}"
    
    # Framing
    elif tool_name == "nina_framing_get_info":
        return "framing/info"
    elif tool_name == "nina_framing_set_source":
        source = args.get('source', '')
        return f"framing/set-source?source={source}"
    elif tool_name == "nina_framing_slew":
        return "framing/slew"
    
    # Utility
    elif tool_name == "nina_time_now":
        return "time/now"
    elif tool_name == "nina_wait":
        seconds = args.get('seconds', 1)
        return f"time/wait?seconds={seconds}"
    
    return None


async def main():
    """Run the MCP server"""
    logger.info("Starting NINA Advanced API MCP Server...")
    
    async with stdio_server() as (read_stream, write_stream):
        await server.run(
            read_stream,
            write_stream,
            InitializationOptions(
                server_name="nina-advanced-api-server",
                server_version="1.0.0",
                capabilities=server.get_capabilities(
                    notification_options=NotificationOptions(),
                    experimental_capabilities={},
                )
            )
        )


if __name__ == "__main__":
    asyncio.run(main())
