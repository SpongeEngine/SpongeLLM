using SpongeEngine.KoboldSharp;
using SpongeEngine.LLMSharp.Core;
using SpongeEngine.LLMSharp.Core.Interfaces;
using SpongeEngine.LLMSharp.Core.Models;
using SpongeEngine.LMStudioSharp;
using SpongeEngine.OobaboogaSharp;

namespace SpongeEngine.SpongeLLM
{
    public class SpongeLLMClient: LlmClientBase, ITextCompletion, IStreamableTextCompletion
    {
        public LlmClientBase Client { get; private set; }
        
        public SpongeLLMClient(LlmClientBaseOptions options): base(options) {
            Client = options switch {
                OobaboogaSharpClientOptions oobaboogaOptions => new OobaboogaSharpClient(oobaboogaOptions),
                KoboldSharpClientOptions koboldOptions => new KoboldSharpClient(koboldOptions),
                LmStudioClientOptions lmsOptions => new LmStudioSharpClient(lmsOptions),
                _ => throw new ArgumentException($"Unsupported options type: {options.GetType()}")
            };
        }

        public Task<TextCompletionResult> CompleteTextAsync(TextCompletionRequest request, CancellationToken ct = new CancellationToken())
        {
            if (Client is ITextCompletion completionService)
            {
                return completionService.CompleteTextAsync(request, ct);
            }
    
            throw new NotSupportedException($"Client {Client.GetType()} does not support completions");
        }

        public IAsyncEnumerable<TextCompletionToken> CompleteTextStreamAsync(TextCompletionRequest request, CancellationToken ct = new CancellationToken())
        {
            if (Client is IStreamableTextCompletion completionService)
            {
                return completionService.CompleteTextStreamAsync(request, ct);
            }
    
            throw new NotSupportedException($"Client {Client.GetType()} does not support streaming completions");
        }
    }
}