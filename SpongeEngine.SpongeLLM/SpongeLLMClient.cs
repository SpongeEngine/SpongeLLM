using Microsoft.Extensions.Logging;
using SpongeEngine.KoboldSharp;
using SpongeEngine.LMStudioSharp;
using SpongeEngine.OobaboogaSharp;
using SpongeEngine.SpongeLLM.Core;
using SpongeEngine.SpongeLLM.Core.Interfaces;
using SpongeEngine.SpongeLLM.Core.Models;

namespace SpongeEngine.SpongeLLM
{
    public class SpongeLLMClient: LLMClientBase, IIsAvailable, ITextCompletion, IStreamableTextCompletion
    {
        public LLMClientBase Client { get; set; }
        
        public SpongeLLMClient(LLMClientBaseOptions options): base(options) {
            Client = options switch {
                OobaboogaSharpClientOptions oobaboogaOptions => new OobaboogaSharpClient(oobaboogaOptions),
                KoboldSharpClientOptions koboldOptions => new KoboldSharpClient(koboldOptions),
                LMStudioClientOptions lmsOptions => new LMStudioSharpClient(lmsOptions),
                _ => throw new ArgumentException($"Unsupported options type: {options.GetType()}")
            };
        }
        
        public virtual async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await Options.HttpClient.GetAsync(Options.HttpClient.BaseAddress, cancellationToken);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Options.Logger?.LogWarning(ex, "Availability check failed");
                return false;
            }
        }

        public Task<TextCompletionResult> CompleteTextAsync(TextCompletionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (Client is not ITextCompletion clientWithTextCompletion)
            {
                throw new NotSupportedException($"{Client.GetType()} does not support {nameof(ITextCompletion)}");
            }
            
            return clientWithTextCompletion.CompleteTextAsync(request, cancellationToken);
        }

        public IAsyncEnumerable<TextCompletionToken> CompleteTextStreamAsync(TextCompletionRequest request, CancellationToken cancellationToken = new CancellationToken())
        {
            if (Client is not IStreamableTextCompletion clientWithStreamableTextCompletion)
            {
                throw new NotSupportedException($"{Client.GetType()} does not support {nameof(IStreamableTextCompletion)}");
            }
            
            return clientWithStreamableTextCompletion.CompleteTextStreamAsync(request, cancellationToken);
        }
    }
}