using System.Reflection;
using System.Runtime.InteropServices;

// General Information
[assembly: AssemblyTitle("AI Assistant")]
[assembly: AssemblyDescription("Multi-provider AI assistant with MCP equipment control, dynamic model discovery, image analysis, and extensible tool framework for intelligent astrophotography automation")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Michele Bergo")]
[assembly: AssemblyProduct("NINA AI Assistant Plugin")]
[assembly: AssemblyCopyright("Copyright © 2026 Michele Bergo")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// COM visibility
[assembly: ComVisible(false)]

// Plugin GUID - Must match manifest.json Identifier
[assembly: Guid("af5e2826-e3b4-4b9c-9a1a-1e8d7c8b6a9e")]

// Version information
[assembly: AssemblyVersion("2.1.0.0")]
[assembly: AssemblyFileVersion("2.1.0.0")]

// Plugin metadata
[assembly: AssemblyMetadata("MinimumApplicationVersion", "3.0.0.0")]
[assembly: AssemblyMetadata("License", "MIT")]
[assembly: AssemblyMetadata("LicenseURL", "https://github.com/michelebergo/nina-ai-assistant/blob/main/LICENSE")]
[assembly: AssemblyMetadata("Repository", "https://github.com/michelebergo/nina-ai-assistant")]
[assembly: AssemblyMetadata("FeaturedImageURL", "https://raw.githubusercontent.com/michelebergo/nina-ai-assistant/main/icon.png")]
[assembly: AssemblyMetadata("ChangelogURL", "https://github.com/michelebergo/nina-ai-assistant/blob/main/CHANGELOG.md")]
[assembly: AssemblyMetadata("Tags", "ai,assistant,chat,mcp,automation")]
[assembly: AssemblyMetadata("LongDescription", @"Your intelligent astrophotography companion - transform NINA into a conversational, context-aware imaging system that understands your goals and helps you achieve better results.

🔭 FOR ASTROPHOTOGRAPHERS:
• Quick Session Setup: 'Set up for M31 tonight' - AI configures equipment, cooling, filters, and exposure settings
• Real-Time Troubleshooting: Analyze failed frames, high HFR, guiding issues, poor focus - get instant suggestions
• Image Quality Feedback: AI reviews your captures, identifies problems (tracking, focus, star bloat), suggests corrections
• Learning Assistant: Ask 'Why is my HFR high?' or 'Best Ha exposure for M42' - get expert guidance while imaging
• Sequence Optimization: 'Plan 4-hour session on Horsehead' - AI suggests optimal filter rotation, dither patterns, exposure times
• Weather-Aware Planning: Integrate weather data to adjust sequences, recommend targets, predict session success

🤖 5 AI PROVIDERS (Free to Advanced):
• GitHub Models (FREE) - No credit card, great for learning
• OpenAI GPT-4o/o1 - Most capable reasoning for complex planning
• Anthropic Claude Sonnet 4.5 - Best for equipment control via MCP
• Google Gemini 2.0 - Fast responses, MCP equipment control support
• Ollama (Local) - Privacy-focused, works offline, no API costs
Dynamic model discovery ensures you always have latest AI capabilities.

🎛️ NATURAL LANGUAGE EQUIPMENT CONTROL (via MCP):
Control your entire observatory through conversation - no clicking through menus:
• Startup: 'Connect camera and mount, cool to -10°C, unpark and go to home position'
• Targeting: 'Slew to Orion Nebula, center it, start guiding'
• Capture: 'Take 5-minute Ha exposure, then switch to OIII filter and repeat'
• Focus: 'Run autofocus routine, use V-curve method'
• Troubleshooting: 'Why is guiding poor? Check calibration and suggest fixes'
100+ built-in MCP tools: cameras, mounts, focusers, filter wheels, rotators, dome control, flat panels, guiding, platesolving, meridian flips, dithering, safety monitors.

📊 INTELLIGENT IMAGING ANALYSIS:
• HFR Monitoring: Tracks focus quality, alerts to drift, suggests refocus timing
• Star Detection: Analyzes star counts, bloating, ellipticity - identifies tracking issues early
• Background Assessment: Detects light pollution, gradients, suggests calibration improvements
• Exposure Evaluation: Compares SNR across filters, recommends optimal integration times
• Pattern Recognition: Identifies systematic issues (periodic error, flexure, wind shake)

🔧 WORKFLOW AUTOMATION:
• Multi-Target Planning: 'Image M31 until it sets, then switch to M33'
• Adaptive Sequences: Adjusts based on conditions - clouds detected? Switch to narrowband
• Equipment Coordination: Synchronizes rotator angle, filter changes, focus offsets automatically
• Error Recovery: Focus failed? AI suggests solutions and retries with adjusted parameters
• Session Management: 'What did I capture tonight?' - Get summary with quality metrics

🛠️ EXTENSIBILITY FOR ADVANCED USERS:
Connect external Python MCP servers to add:
• Astronomy Calculations: Optimal imaging times, transit predictions, moon interference
• Weather Integration: Real-time cloud forecasts, seeing predictions, transparency data
• Catalog Queries: Search SIMBAD/NED, plan mosaic panels, find guide stars
• Personal Databases: Track object history, successful settings, equipment performance
• Custom Automation: Telescope-specific routines, park positions, startup/shutdown sequences

💡 BEGINNER-FRIENDLY:
• Start with free GitHub Models - no payment info needed
• Ask questions while learning: 'What's a good starting point for M42?'
• Get explanations: 'Why do I need dark frames?'
• Guided troubleshooting for common issues
• Conversational interface - no complex syntax to learn

⚡ PRO FEATURES:
• Streaming responses - see answers as they're generated
• Context-aware conversations - AI remembers your equipment, preferences, recent issues
• Clean UI - only shows settings for your selected AI provider
• Auto-updates - new models appear automatically without plugin updates
• Comprehensive logging - track all AI interactions for troubleshooting
• Multiple simultaneous providers - compare responses from different AIs

Transform complex equipment control and imaging workflows into simple conversations. Focus on capturing amazing data while AI handles coordination, troubleshooting, and optimization.")]
