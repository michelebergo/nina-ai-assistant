# Comprehensive MCP Tools Implementation Plan

This document outlines all tools from the Python MCP server that need to be added to the C# implementation.

## Currently Implemented (40 tools)
✅ nina_get_status
✅ nina_get_version
✅ Camera: connect, disconnect, info, list, capture, cooling, abort, statistics
✅ Mount: connect, disconnect, info, list, slew, park, unpark, home, stop_slew, tracking
✅ Focuser: connect, disconnect, info, list, move
✅ Filter Wheel: connect, disconnect, info, list, change
✅ Guider: connect, disconnect, info, list, start, stop, calibrate
✅ Dome: connect, disconnect, info, list, open/close shutter, park, slew
✅ Image History
✅ Application: switch_tab, get_plugins
✅ Flats: start, stop, get_status

## Missing Tools to Implement (60+ tools)

### Rotator (8 tools)
- nina_connect_rotator
- nina_disconnect_rotator
- nina_list_rotator_devices
- nina_get_rotator_info
- nina_move_rotator
- nina_halt_rotator
- nina_sync_rotator
- nina_set_rotator_reverse

### Flat Panel (8 tools)
- nina_connect_flatpanel
- nina_disconnect_flatpanel
- nina_list_flatpanel_devices
- nina_get_flatpanel_info
- nina_set_flatpanel_light
- nina_set_flatpanel_cover
- nina_set_flatpanel_brightness

### Switch (6 tools)
- nina_connect_switch
- nina_disconnect_switch
- nina_list_switch_devices
- nina_get_switch_channels
- nina_set_switch

### Weather (5 tools)
- nina_connect_weather
- nina_disconnect_weather
- nina_get_weather_info
- nina_list_weather_sources
- nina_rescan_weather_sources

### Safety Monitor (5 tools)
- nina_connect_safetymonitor
- nina_disconnect_safetymonitor
- nina_get_safetymonitor_info
- nina_list_safetymonitor_devices
- nina_rescan_safetymonitor_devices

### Advanced Camera (10 tools)
- nina_set_binning
- nina_control_dew_heater
- nina_set_readout_mode
- nina_set_camera_gain
- nina_set_camera_offset
- nina_set_camera_usb_limit
- nina_start_warming
- nina_stop_warming
- nina_list_camera_devices (already have)
- nina_rescan_camera_devices

### Advanced Mount (5 tools)
- nina_sync_mount
- nina_set_park_position
- nina_rescan_mount_devices

### Advanced Focuser (5 tools)
- nina_start_autofocus
- nina_cancel_autofocus
- nina_get_autofocus_status
- nina_halt_focuser
- nina_rescan_focuser_devices

### Sequence (10 tools)
- nina_sequence_start
- nina_sequence_stop
- nina_sequence_load
- nina_sequence_json
- nina_sequence_state
- nina_sequence_list_available
- nina_sequence_edit
- nina_sequence_reset
- nina_sequence_set_target

### Plate Solving (5 tools)
- nina_platesolve_capsolve
- nina_platesolve_sync
- nina_platesolve_center
- nina_platesolve_status
- nina_platesolve_cancel

### Advanced Flats (7 tools)
- nina_sky_flats
- nina_panel_flats
- nina_auto_brightness_flats
- nina_auto_exposure_flats
- nina_trained_flats
- nina_trained_dark_flat
- nina_get_flats_progress

### Image Operations (8 tools)
- nina_get_image
- nina_get_prepared_image
- nina_solve_image
- nina_solve_prepared_image
- nina_get_image_parameter
- nina_set_image_parameter

### Framing Assistant (7 tools)
- nina_get_framingassistant_info
- nina_set_framingassistant_source
- nina_set_framingassistant_coordinates
- nina_framingassistant_slew
- nina_set_framingassistant_rotation
- nina_determine_framingassistant_rotation
- nina_get_moon_separation

### Profile (3 tools)
- nina_get_profile
- nina_change_profile_value
- nina_get_horizon

### Livestack (5 tools)
- nina_get_livestack_status
- nina_start_livestack
- nina_stop_livestack
- nina_get_livestack_available_stacks
- nina_get_livestack_stacked_image

### Utility (3 tools)
- nina_time_now
- nina_wait
- nina_get_start_time

## Implementation Priority

### High Priority (Essential for most users)
1. Rotator (common equipment)
2. Flat Panel (flat calibration)
3. Sequence operations (automation)
4. Plate solving (goto accuracy)
5. Advanced camera (gain/offset/binning)
6. Autofocus (critical for imaging)

### Medium Priority (Advanced users)
1. Switch (observatory automation)
2. Weather station
3. Safety monitor
4. Framing assistant
5. Profile management
6. Advanced flats

### Low Priority (Special cases)
1. Livestack
2. Image parameters
3. Utility functions

## Total Tools Count
- Current: 40 tools
- After full implementation: 100+ tools
- Matches Python MCP server feature parity
