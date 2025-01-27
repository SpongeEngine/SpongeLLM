using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Integration;

public class OllamaSharpIntegrationTests: IntegrationTestsBase
{
    public OllamaSharpIntegrationTests(ITestOutputHelper output) : base(output)
    {
        //Client = new SpongeLLMClient(new LMStudioClientOptions());
    }
}