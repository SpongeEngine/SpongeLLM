using Xunit.Abstractions;
using SpongeEngine.LMStudioSharp;

namespace SpongeEngine.SpongeLLM.Tests.Integration
{
    public class LMStudioSharpIntegrationTests: IntegrationTestsBase
    {
        public LMStudioSharpIntegrationTests(ITestOutputHelper output) : base(output)
        {
            Client = new SpongeLLMClient(new LMStudioClientOptions());
        }
    }
}