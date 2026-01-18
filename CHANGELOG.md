# Changelog

All notable changes to AI Assistant plugin for NINA will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0.1] - 2026-01-18

### Added
- 🎉 **Initial release**

#### Multi-Provider AI Support
- GitHub Models (free)
- OpenAI (GPT-4o, GPT-4o-mini)
- Anthropic Claude (with MCP support)
- Google Gemini (free tier available)
- Ollama (local, free)
- OpenRouter (pay-per-use)

#### MCP Equipment Control (Anthropic Claude)
- Camera control: connect, capture, cooling, abort
- Mount control: slew, park/unpark, tracking modes
- Focuser control: move to position
- Filter wheel control: change filters
- Guider control: start/stop guiding, calibration
- Dome control: open/close shutter, park
- 50+ equipment control tools via NINA Advanced API

#### User Interface
- Interactive chat panel (dockable)
- Provider selection with status indicator
- Secure API key storage
- MCP connection testing
- Real-time equipment status queries

#### Settings & Configuration
- Persistent settings across sessions
- Per-provider model selection
- MCP host/port configuration (default: localhost:1888)
- Connection testing for all providers

### Technical Details
- Built for NINA 3.x (.NET 8.0)
- Uses NINA Advanced API v2 for MCP integration
- Supports iterative tool calling for complex operations
- Settings stored securely in NINA's local user profile

[1.0.0.1]: https://github.com/michelebergo/nina-ai-assistant/releases/tag/v1.0.0.1
