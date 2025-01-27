using Xunit.Abstractions;
using SpongeEngine.KoboldSharp;

namespace SpongeEngine.SpongeLLM.Tests.Integration
{
    public class KoboldSharpIntegrationTests: IntegrationTestsBase
    {
        public KoboldSharpIntegrationTests(ITestOutputHelper output) : base(output)
        {
            Client = new SpongeLLMClient(new KoboldSharpClientOptions());
        }
    }
}