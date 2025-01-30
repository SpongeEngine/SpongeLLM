using FluentAssertions;
using Microsoft.Extensions.Logging;
using SpongeEngine.KoboldSharp;
using SpongeEngine.LMStudioSharp;
using SpongeEngine.OobaboogaSharp;
using SpongeEngine.SpongeLLM.Core;
using SpongeEngine.SpongeLLM.Core.Interfaces;
using SpongeEngine.SpongeLLM.Core.Models;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using Xunit;
using Xunit.Abstractions;
using Moq;

namespace SpongeEngine.SpongeLLM.Tests.Unit
{
    public class UnitTests : UnitTestBase
    {
        private readonly Mock<ILogger<SpongeLLM.SpongeLLMClient>> _mockLogger;
        private readonly HttpClient _httpClient;
        public UnitTests(ITestOutputHelper output) : base(output)
        {
            _mockLogger = new Mock<ILogger<SpongeLLM.SpongeLLMClient>>();
            _httpClient = new HttpClient { BaseAddress = new Uri(Server.Urls[0]) };
        }

        [Theory]
        [InlineData(typeof(OobaboogaSharpClientOptions))]
        [InlineData(typeof(KoboldSharpClientOptions))]
        [InlineData(typeof(LMStudioClientOptions))]
        public void Constructor_InitializesCorrectClientType(Type optionsType)
        {
            // Arrange
            var options = (LLMClientBaseOptions)Activator.CreateInstance(optionsType)!;
            options.HttpClient = _httpClient;
            options.Logger = _mockLogger.Object;

            // Act
            var client = new SpongeLLM.SpongeLLMClient(options);

            // Assert
            client.Should().NotBeNull();
            client.Client.Should().BeOfType(GetExpectedClientType(optionsType));
            client.Client.Options.Should().BeSameAs(options);
            client.Client.Options.HttpClient.Should().BeSameAs(_httpClient);
            client.Client.Options.Logger.Should().BeSameAs(_mockLogger.Object);
        }

        [Fact]
        public void Constructor_ThrowsForUnsupportedOptions()
        {
            // Arrange
            var options = new SpongeLLMClientOptions
            {
                HttpClient = _httpClient,
                Logger = _mockLogger.Object
            };

            // Act & Assert
            var act = () => new SpongeLLM.SpongeLLMClient(options);
            act.Should().Throw<ArgumentException>()
                .WithMessage($"Unsupported options type: {typeof(SpongeLLMClientOptions)}");
        }

        [Fact]
        public async Task IsAvailableAsync_ReturnsTrue_WhenServerResponds()
        {
            // Arrange
            Server
                .Given(Request.Create().WithPath("/").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(200));

            var options = new OobaboogaSharpClientOptions
            {
                HttpClient = _httpClient,
                Logger = _mockLogger.Object
            };
            var client = new SpongeLLM.SpongeLLMClient(options);

            // Act
            var result = await client.IsAvailableAsync();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task IsAvailableAsync_ReturnsFalse_WhenServerFails()
        {
            // Arrange
            var loggerFactory = LoggerFactory.Create(builder => builder.AddXUnit(Output));
            var logger = loggerFactory.CreateLogger<SpongeLLM.SpongeLLMClient>();

            Server
                .Given(Request.Create().WithPath("/").UsingGet())
                .RespondWith(Response.Create().WithStatusCode(500));

            var options = new OobaboogaSharpClientOptions
            {
                HttpClient = _httpClient,
                Logger = logger
            };
            var client = new SpongeLLM.SpongeLLMClient(options);

            // Act
            var result = await client.IsAvailableAsync();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task CompleteTextAsync_ThrowsWhenClientDoesNotSupportCompletions()
        {
            // Arrange
            var mockClient = new Mock<LLMClientBase>(new KoboldSharpClientOptions()) { CallBase = true };
            mockClient.As<ITextCompletion>()
                .Setup(x => x.CompleteTextAsync(It.IsAny<TextCompletionRequest>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new NotSupportedException("Client does not support completions"));

            var client = new TestSpongeLlmClient(new KoboldSharpClientOptions(), mockClient.Object);

            // Act
            Func<Task> act = () => client.CompleteTextAsync(new TextCompletionRequest());

            // Assert
            await act.Should().ThrowAsync<NotSupportedException>()
                .WithMessage("Client does not support completions");
        }

        [Fact]
        public async Task CompleteTextStreamAsync_ThrowsWhenClientDoesNotSupportStreaming()
        {
            // Arrange
            var mockClient = new Mock<LLMClientBase>(new KoboldSharpClientOptions()) { CallBase = true };
            mockClient.As<IStreamableTextCompletion>()
                .Setup(x => x.CompleteTextStreamAsync(It.IsAny<TextCompletionRequest>(), It.IsAny<CancellationToken>()))
                .Throws(new NotSupportedException("Client does not support streaming completions"));

            var client = new TestSpongeLlmClient(new KoboldSharpClientOptions(), mockClient.Object);

            // Act
            Func<Task> act = async () => await client.CompleteTextStreamAsync(new TextCompletionRequest()).ToListAsync();

            // Assert
            await act.Should().ThrowAsync<NotSupportedException>().WithMessage("Client does not support streaming completions");
        }

        private static Type GetExpectedClientType(Type optionsType)
        {
            return optionsType switch
            {
                Type t when t == typeof(OobaboogaSharpClientOptions) => typeof(OobaboogaSharpClient),
                Type t when t == typeof(KoboldSharpClientOptions) => typeof(KoboldSharpClient),
                Type t when t == typeof(LMStudioClientOptions) => typeof(LMStudioSharpClient),
                _ => throw new ArgumentException($"Unexpected options type: {optionsType}")
            };
        }
    }

    // Test helper class to allow injection of mock client
    internal class TestSpongeLlmClient : SpongeLLM.SpongeLLMClient
    {
        public TestSpongeLlmClient(LLMClientBaseOptions options, LLMClientBase client) : base(options)
        {
            Client = client;
        }
    }
}