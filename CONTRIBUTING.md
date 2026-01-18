# Contributing to NINA AI Assistant

Thank you for your interest in contributing to the NINA AI Assistant plugin! 🎉

## How to Contribute

### Reporting Bugs

1. Check if the bug has already been reported in [Issues](../../issues)
2. If not, create a new issue with:
   - Clear title and description
   - Steps to reproduce
   - Expected vs actual behavior
   - NINA version and plugin version
   - Log files if relevant

### Suggesting Features

1. Open an issue with the `enhancement` label
2. Describe the feature and its use case
3. Explain how it benefits astrophotographers

### Pull Requests

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Make your changes
4. Test thoroughly with NINA
5. Commit with clear messages: `git commit -m "Add feature X"`
6. Push to your fork: `git push origin feature/my-feature`
7. Open a Pull Request

## Development Setup

### Prerequisites

- Visual Studio 2022 or later
- .NET 8.0 SDK
- NINA 3.x installed

### Building

```bash
git clone https://github.com/michelebergo/NINA_AI_plugin.git
cd NINA_AI_plugin
dotnet build -c Release
```

### Testing

1. Build the plugin
2. Start NINA
3. Test your changes in the AI Assistant panel
4. Check NINA logs for errors

## Code Style

- Follow C# coding conventions
- Use meaningful variable and method names
- Add XML documentation for public methods
- Keep methods focused and small

## Questions?

Open an issue or start a discussion. We're happy to help!
