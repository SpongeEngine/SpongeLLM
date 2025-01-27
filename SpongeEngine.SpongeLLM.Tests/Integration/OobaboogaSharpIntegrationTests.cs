using SpongeEngine.OobaboogaSharp;
using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Integration;

public class OobaboogaSharpIntegrationTests: IntegrationTestsBase
{
    public OobaboogaSharpIntegrationTests(ITestOutputHelper output) : base(output)
    {
        Client = new SpongeLLMClient(new OobaboogaSharpClientOptions());
    }
}