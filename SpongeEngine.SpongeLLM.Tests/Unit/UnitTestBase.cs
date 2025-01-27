using Microsoft.Extensions.Logging;
using SpongeEngine.KoboldSharp;
using SpongeEngine.SpongeLLM.Tests.Common;
using WireMock.Server;
using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Unit
{
    public abstract class UnitTestBase : TestBase
    {
        protected readonly WireMockServer Server;
        protected readonly KoboldSharpClient Client;
        protected ITestOutputHelper Output;
        
        protected UnitTestBase(ITestOutputHelper output)
        {
            Output = output;
            Server = WireMockServer.Start();
            Client = new KoboldSharpClient(
                new KoboldSharpClientOptions() 
                {
                    HttpClient = new HttpClient
                    {
                        BaseAddress = new Uri(Server.Urls[0])
                    },
                    Logger = LoggerFactory
                        .Create(builder => builder.AddXUnit(output))
                        .CreateLogger(GetType()),
                });
        }
    }
}