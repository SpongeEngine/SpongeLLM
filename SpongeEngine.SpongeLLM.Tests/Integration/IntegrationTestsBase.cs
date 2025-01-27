using SpongeEngine.KoboldSharp;
using SpongeEngine.SpongeLLM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Integration
{
    public abstract class IntegrationTestsBase : TestBase, IAsyncLifetime
    {
        protected SpongeLLMClient? Client;
        protected ITestOutputHelper Output;
        protected bool ServerAvailable;

        protected IntegrationTestsBase(ITestOutputHelper output)
        {
            Output = output;
        }

        public async Task InitializeAsync()
        {
            try
            {
                ServerAvailable = await Client.IsAvailableAsync();
                
                if (ServerAvailable)
                {
                    Output.WriteLine("Server is available");
                }
                else
                {
                    Output.WriteLine("Server is not available");
                    throw new SkipException("Server is not available");
                }
            }
            catch (Exception ex) when (ex is not SkipException)
            {
                Output.WriteLine($"Failed to connect to server: {ex.Message}");
                throw new SkipException("Failed to connect to server");
            }
        }

        public async Task DisposeAsync()
        {
            Client?.Options.HttpClient?.Dispose();
            
            await Task.CompletedTask;
        }
    }
}