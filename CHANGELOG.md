# NINA AI Assistant - Changelog

All notable changes to this project will be documented in this file.

## [1.0.0.1] - 2026-01-18

### Added
- Initial release of NINA AI Assistant Plugin
- Multi-provider AI support:
  - GitHub Models (free tier available)
  - Microsoft Foundry (formerly Azure AI Foundry)
  - Azure OpenAI Service
  - OpenAI direct API
  - Anthropic Claude
  - Google AI (Gemini)
- AI-powered sequence items:
  - AI Image Analysis - Automated quality assessment
  - AI Suggest Exposure - Intelligent exposure recommendations
  - AI Query Assistant - Interactive Q&A
- Comprehensive settings UI for all providers
- Support for 40+ AI models across providers
- Secure API key storage in NINA profiles
- Connection testing for all providers

### Features
- **Free tier support** with GitHub Models and Google AI
- **Intelligent image analysis** with customizable prompts
- **Smart exposure suggestions** based on target and conditions
- **Natural language queries** for astrophotography advice
- **Multi-provider switching** - use different AIs for different tasks
- **Model selection** - choose the best model for your needs

### Technical
- Built on .NET 8.0
- Compatible with NINA 3.0+
- Uses Azure AI Inference SDK for unified API access
- Implements proper MEF exports for NINA integration
- Async/await throughout for responsive UI

### Documentation
- Comprehensive README with setup guides
- Quick start for GitHub Models (free)
- Configuration guides for all providers
- Cost considerations and recommendations
- Troubleshooting section

## [Unreleased]

### Planned Features
- Image upload for vision models (GPT-4o, Claude, Gemini)
- Historical session analysis
- Automated sequence optimization
- Weather-based recommendations
- Equipment-specific tuning suggestions
- Multi-image batch analysis
- Custom AI prompts library
- Integration with framing assistant
- Plate solving validation with AI
- Target recommendation engine

---

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).
