using SpongeEngine.SpongeLLM.Core.Interfaces;
using SpongeEngine.SpongeLLM.Core.Models;
using SpongeEngine.SpongeLLM.Tests.Common;
using Xunit;
using Xunit.Abstractions;

namespace SpongeEngine.SpongeLLM.Tests.Integration
{
    public abstract class IntegrationTestsBase : TestBase, IAsyncLifetime
    {
        protected SpongeLLM.SpongeLLMClient? Client;
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
                ServerAvailable = await Client!.IsAvailableAsync();
                
                if (ServerAvailable)
                {
                    Output.WriteLine("Server is available");
                }
                else
                {
                    throw new SkipException("Server is not available");
                }
            }
            catch (Exception ex) when (ex is not SkipException)
            {
                Output.WriteLine($"Failed to connect: {ex.Message}");
                throw new SkipException("Connection failed");
            }
        }

        public async Task DisposeAsync()
        {
            Client?.Options.HttpClient?.Dispose();
            await Task.CompletedTask;
        }

        [SkippableFact]
        public async Task CompleteTextAsync_ReturnsValidResponse()
        {
            Skip.IfNot(ServerAvailable);
            Skip.If(Client!.Client is not ITextCompletion, 
                $"{Client.Client.GetType().Name} doesn't support completions");
            
            var result = await Client.CompleteTextAsync(new TextCompletionRequest { Prompt = "Hello" });
            Assert.False(string.IsNullOrEmpty(result.Text));
        }

        [SkippableFact]
        public async Task CompleteTextStreamAsync_ReturnsStream()
        {
            Skip.IfNot(ServerAvailable);
            Skip.If(Client!.Client is not IStreamableTextCompletion,
                $"{Client.Client.GetType().Name} doesn't support streaming");
            
            await foreach (var token in Client.CompleteTextStreamAsync(new TextCompletionRequest { Prompt = "Hello" }))
            {
                Assert.NotNull(token.Text);
                break;
            }
        }
    }
}