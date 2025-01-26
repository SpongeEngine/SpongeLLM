# SpongeLLM (In Progress)
[![NuGet](https://img.shields.io/nuget/v/LLMSharp.svg)](https://www.nuget.org/packages/LLMSharp)
[![NuGet Downloads](https://img.shields.io/nuget/dt/LLMSharp.svg)](https://www.nuget.org/packages/LLMSharp)
[![Tests](https://github.com/SpongeEngine/LMSharp/actions/workflows/test.yml/badge.svg)](https://github.com/SpongeEngine/LMSharp/actions/workflows/test.yml)
[![License](https://img.shields.io/github/license/SpongeEngine/LLMSharp)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0%2B-512BD4)](https://dotnet.microsoft.com/download)

Unified C# client for LLM providers.

- Single API: Use the same code regardless of the underlying LLM provider
- Provider Flexibility: Easily switch between KoboldCpp, Ollama, LM Studio, or Text Generation WebUI
- Modern .NET: Async/await, streaming support, and comprehensive logging

📦 [View Package on NuGet](https://www.nuget.org/packages/LLMSharp)

## Feature Comparison
| Feature | LLMSharp | OpenAI.NET | LLamaSharp | OllamaSharp |
|---------|-------------|------------|------------|-------------|
| Local LLM Support | ✅ | ❌ | ✅ | ✅ |
| Multiple Providers | ✅ | ❌ | ❌ | ❌ |
| KoboldCpp Support | ✅ | ❌ | ❌ | ❌ |
| Ollama Support | ✅ | ❌ | ❌ | ✅ |
| LM Studio Support | ✅ | ❌ | ❌ | ❌ |
| Text Gen WebUI Support | ✅ | ❌ | ❌ | ❌ |
| Streaming | ✅ | ✅ | ✅ | ✅ |
| OpenAI Compatible | ✅ | ✅ | ❌ | ✅ |
| Progress Tracking | ✅ | ❌ | ❌ | ❌ |
| Retry Policies | ✅ | ❌ | ❌ | ❌ |
| Circuit Breaker | ✅ | ❌ | ❌ | ❌ |
| .NET Standard 2.0 | ❌ | ✅ | ✅ | ✅ |
| .NET 6.0+ | ✅ | ✅ | ✅ | ✅ |

```mermaid
classDiagram
    %% Core Interfaces
    class ICompletionService {
        <<interface>>
        +CompleteAsync(CompletionRequest)*
        +StreamCompletionAsync(CompletionRequest)*
    }
    class IChatService {
        <<interface>>
        +ChatCompleteAsync(ChatRequest)*
        +StreamChatAsync(ChatRequest)*
    }
    class IModelMetadata {
        <<interface>>
        +GetAvailableModelsAsync()*
        +GetModelInfoAsync(string)*
    }

    %% Base Abstract Client
    class LlmClient {
        <<abstract>>
        #HttpClient _httpClient
        #ILogger _logger
        #LlmOptions _options
        #RetryPolicy _retryPolicy
        #CircuitBreaker _circuitBreaker
        #ExecuteWithResilienceAsync[T]()
        +IsAvailableAsync()
    }

    %% Providers 
    class OobaboogaSharp {
        +CompleteAsync()
        +StreamCompletionAsync()
        +ChatCompleteAsync()
        +GetAvailableModels()
    }
    class LmStudioSharp {
        +CompleteAsync()
        +StreamCompletionAsync()
        +ChatCompleteAsync()
    }
    class KoboldSharp {
        +CompleteAsync()
        +StreamCompletionAsync()
    }
    class Gpt4AllSharp {
        +ChatCompleteAsync()
        +StreamChatAsync()
        +GetAvailableModels()
    }

    %% Base Inheritance
    LlmClient --> OobaboogaSharp
    LlmClient --> LmStudioSharp
    LlmClient --> KoboldSharp
    LlmClient --> Gpt4AllSharp

    %% Interface Implementation
    ICompletionService <.. OobaboogaSharp
    IChatService <.. OobaboogaSharp
    IModelMetadata <.. OobaboogaSharp

    ICompletionService <.. LmStudioSharp
    IChatService <.. LmStudioSharp

    ICompletionService <.. KoboldSharp

    IChatService <.. Gpt4AllSharp
    IModelMetadata <.. Gpt4AllSharp
```

## Supported Providers
- [KoboldCpp](https://github.com/LostRuins/koboldcpp): Both native and OpenAI-compatible modes
- [Ollama](https://github.com/ollama/ollama): Run Llama 2, Code Llama, and other models locally (using OllamaSharp).
- [LM Studio](https://lmstudio.ai): Local deployment of various open-source models
- [Text Generation WebUI](https://github.com/oobabooga/text-generation-webui): Popular web interface for running local models

## Installation
Install LLMSharp via NuGet:
```bash
dotnet add package LLMSharp
```

## Quick Start
```csharp
using LLMSharp.Client;
using LLMSharp.Models.Configuration;

// Create client with KoboldCpp provider
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:5000",
    ProviderOptions = new KoboldCppNativeOptions
    {
        ContextSize = 2048,
        UseGpu = true,
        RepetitionPenalty = 1.1f
    }
};

using var client = new LocalAIClient(options);

// Generate text completion
string response = await client.CompleteAsync("Write a short story about a robot:");

// Stream completion tokens
await foreach (var token in client.StreamCompletionAsync("Once upon a time..."))
{
    Console.Write(token);
}

// List available models
var models = await client.GetAvailableModelsAsync();
foreach (var model in models)
{
    Console.WriteLine($"Model: {model.Name} (Provider: {model.Provider})");
    Console.WriteLine($"Context Length: {model.Capabilities.MaxContextLength}");
}
```

## Provider Configuration

### KoboldCpp (Native)
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:5000",
    ProviderOptions = new KoboldCppNativeOptions
    {
        ContextSize = 2048,
        UseGpu = true,
        RepetitionPenalty = 1.1f,
        RepetitionPenaltyRange = 320,
        TrimStop = true,
        Mirostat = new MirostatSettings
        {
            Mode = 2,
            Tau = 5.0f,
            Eta = 0.1f
        }
    }
};
```

### KoboldCpp (OpenAI-compatible)
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:5000",
    ProviderOptions = new KoboldCppOpenAiOptions
    {
        ContextSize = 2048,
        UseGpu = true,
        ModelName = "koboldcpp",
        UseChatCompletions = true
    }
};
```

### Ollama
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:11434",
    ProviderOptions = new OllamaOptions
    {
        ConcurrentRequests = 1
    }
};
```

### LM Studio
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:1234",
    ProviderOptions = new LMStudioOptions
    {
        UseOpenAIEndpoint = true
    }
};
```

### Text Generation WebUI
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:7860",
    ProviderOptions = new TextGenWebOptions
    {
        UseOpenAIEndpoint = true
    }
};
```

## Completion Options
```csharp
var options = new CompletionOptions
{
    ModelName = "wizardLM",         // Optional model name
    MaxTokens = 200,                // Max tokens to generate
    Temperature = 0.7f,             // Randomness (0.0-1.0)
    TopP = 0.9f,                    // Nucleus sampling threshold
    StopSequences = new[] { "\n" }  // Sequences that stop generation
};

string response = await client.CompleteAsync("Your prompt here", options);
```

## Progress Tracking
```csharp
client.OnProgress += (progress) =>
{
    switch (progress.State)
    {
        case LocalAIProgressState.Starting:
            Console.WriteLine("Starting completion...");
            break;
        case LocalAIProgressState.Processing:
            Console.WriteLine($"Processing: {progress.Message}");
            break;
        case LocalAIProgressState.Streaming:
            Console.WriteLine("Receiving tokens...");
            break;
        case LocalAIProgressState.Complete:
            Console.WriteLine("Completion finished!");
            break;
        case LocalAIProgressState.Failed:
            Console.WriteLine($"Error: {progress.Message}");
            break;
    }
};
```

## Error Handling
```csharp
try
{
    var response = await client.CompleteAsync("Test prompt");
}
catch (LocalAIException ex)
{
    Console.WriteLine($"LocalAI API error: {ex.Message}");
    if (ex.StatusCode.HasValue)
    {
        Console.WriteLine($"Status code: {ex.StatusCode}");
    }
    if (ex.Provider != null)
    {
        Console.WriteLine($"Provider: {ex.Provider}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"General error: {ex.Message}");
}
```

## Advanced Configuration
```csharp
var options = new LocalAIOptions
{
    BaseUrl = "http://localhost:5000",
    ApiKey = "optional_api_key",
    Timeout = TimeSpan.FromMinutes(2),
    MaxRetryAttempts = 3,
    RetryDelay = TimeSpan.FromSeconds(2),
    Logger = loggerInstance,
    JsonSettings = new JsonSerializerSettings(),
    ProviderOptions = new KoboldCppNativeOptions()
};
```

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing
Contributions are welcome! Please feel free to submit a Pull Request.

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for details on:
- How to publish to NuGet
- Development guidelines
- Code style
- Testing requirements
- Pull request process

## Support
For issues and feature requests, please use the [GitHub issues page](https://github.com/SpongeEngine/LLMSharp/issues).
