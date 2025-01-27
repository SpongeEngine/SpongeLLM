# SpongeLLM
[![NuGet](https://img.shields.io/nuget/v/SpongeEngine.SpongeLLM.svg)](https://www.nuget.org/packages/SpongeEngine.SpongeLLM)
[![NuGet Downloads](https://img.shields.io/nuget/dt/SpongeEngine.SpongeLLM.svg)](https://www.nuget.org/packages/SpongeEngine.SpongeLLM)
[![Tests](https://github.com/SpongeEngine/SpongeLLM/actions/workflows/run-tests.yml/badge.svg)](https://github.com/SpongeEngine/SpongeLLM/actions/workflows/run-tests.yml)
[![License](https://img.shields.io/github/license/SpongeEngine/SpongeLLM)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0-512BD4)](https://dotnet.microsoft.com/download)

A unified C# client for interacting with various LLM providers through a consistent interface.

## Key Features

- **Unified Interface**: Write code once, switch providers seamlessly
- **Multiple Provider Support**: Works with KoboldCpp, Ollama, LM Studio, and Text Generation WebUI
- **.NET Modern Features**: Full async/await support with streaming capabilities
- **Cross-Platform**: Runs on any platform supporting .NET 6.0, 7.0, or 8.0
- **Production Ready**: Includes logging, resilience patterns, and comprehensive error handling

## Installation

Install via NuGet:

```bash
dotnet add package SpongeEngine.SpongeLLM
```

## Quick Start

```csharp
using SpongeEngine.SpongeLLM;
using SpongeEngine.KoboldSharp;
using SpongeEngine.SpongeLLM.Core.Models;

// Initialize with your preferred provider
var options = new KoboldSharpClientOptions
{
    BaseUrl = "http://localhost:5000"
};

var client = new SpongeLLMClient(options);

// Check if service is available
bool isAvailable = await client.IsAvailableAsync();

// Basic text completion
var request = new TextCompletionRequest
{
    Prompt = "Write a short story about a robot:",
    MaxTokens = 100
};

var result = await client.CompleteTextAsync(request);
Console.WriteLine(result.Text);

// Stream completion tokens
await foreach (var token in client.CompleteTextStreamAsync(request))
{
    Console.Write(token.Text);
}
```

## Supported Providers

The library includes built-in support for:

- **KoboldCpp**: Local inference with various open-source models
- **Ollama**: Easy deployment of Llama 2, Code Llama, and other models
- **LM Studio**: User-friendly interface for running local models
- **Text Generation WebUI**: Popular interface for model deployment and inference

## Provider Configuration

### KoboldCpp

```csharp
var options = new KoboldSharpClientOptions
{
    BaseUrl = "http://localhost:5000",
    UseGpu = true,
    MaxContextLength = 2048
};
```

### LM Studio

```csharp
var options = new LMStudioClientOptions
{
    BaseUrl = "http://localhost:1234",
    UseOpenAICompat = true
};
```

### Text Generation WebUI (Oobabooga)

```csharp
var options = new OobaboogaSharpClientOptions
{
    BaseUrl = "http://localhost:7860",
    UseOpenAICompat = true
};
```

## Completion Options

Configure text completion requests with various parameters:

```csharp
var request = new TextCompletionRequest
{
    Prompt = "Once upon a time",
    MaxTokens = 200,
    Temperature = 0.7f,
    TopP = 0.9f,
    StopSequences = new[] { "\n\n", "THE END" }
};
```

## Error Handling

The client includes comprehensive error handling:

```csharp
try
{
    var result = await client.CompleteTextAsync(request);
}
catch (Exception ex) when (ex is NotSupportedException)
{
    Console.WriteLine("This provider doesn't support text completion");
}
catch (Exception ex)
{
    Console.WriteLine($"Error during completion: {ex.Message}");
}
```

## Advanced Configuration

SpongeLLM supports additional configuration through provider-specific options:

```csharp
var options = new KoboldSharpClientOptions
{
    BaseUrl = "http://localhost:5000",
    Timeout = TimeSpan.FromMinutes(2),
    RetryCount = 3,
    Logger = loggerInstance,
    // Additional provider-specific settings
};
```

## Architecture

SpongeLLM uses a modular architecture based on interfaces:

- `ITextCompletion`: Basic text completion capabilities
- `IStreamableTextCompletion`: Streaming completion support
- `IIsAvailable`: Service availability checking

Each provider implements these interfaces as needed, allowing for consistent interaction regardless of the underlying service.

## Contributing

Contributions are welcome! Please see our [Contributing Guidelines](CONTRIBUTING.md) for details on:

- Development setup
- Coding standards
- Testing requirements
- Pull request process

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Support

For issues and feature requests, please use the [GitHub issues page](https://github.com/SpongeEngine/SpongeEngine.SpongeLLM/issues).
