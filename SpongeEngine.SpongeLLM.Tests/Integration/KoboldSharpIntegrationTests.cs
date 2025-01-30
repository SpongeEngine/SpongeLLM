using SpongeEngine.KoboldSharp;
using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Integration
{
    public class KoboldSharpIntegrationTests : IntegrationTestsBase
    {
        public KoboldSharpIntegrationTests(ITestOutputHelper output) : base(output)
        {
            Client = new SpongeLLM.SpongeLLMClient(new KoboldSharpClientOptions
            {
                // Provider-specific configuration
                HttpClient = new HttpClient { 
                    BaseAddress = new Uri("http://localhost:5001/api/") 
                }
            });
        }
    }
}