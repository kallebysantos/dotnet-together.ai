using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;

namespace Together.AI.SemanticKernel;

public class TogetherAIService(string ModelId, TogetherAIClient TogetherAI) : ITextCompletion
{
    public IReadOnlyDictionary<string, string> Attributes => new Dictionary<string, string>();

    public AIRequestSettings GetDefaultRequestSettings() => CreateDefaultRequestSettings(ModelId);

    public static AIRequestSettings CreateDefaultRequestSettings(string modelId) =>
        new()
        {
            ModelId = modelId,
            ServiceId = nameof(TogetherAIService),
            ExtensionData = { { "max_tokens", 128 } }
        };

    public async Task<IReadOnlyList<ITextResult>> GetCompletionsAsync(
        string text,
        AIRequestSettings? requestSettings = null,
        CancellationToken cancellationToken = default
    )
    {
        requestSettings ??= GetDefaultRequestSettings();

        var requestArgs = requestSettings.ToTogetherArgs() with
        {
            Prompt = text,
            StreamTokens = false
        };

        var response = await TogetherAI.GetCompletionResponseAsync(requestArgs, cancellationToken);

        var completion = new TogetherAICompletion(response);

        return await Task.FromResult<IReadOnlyList<ITextResult>>(
            new List<ITextResult> { new TogetherAITextStreamingResult(completion) }
        );
    }

    public async IAsyncEnumerable<ITextStreamingResult> GetStreamingCompletionsAsync(
        string text,
        AIRequestSettings? requestSettings = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        requestSettings ??= GetDefaultRequestSettings();

        var requestArgs = requestSettings.ToTogetherArgs() with
        {
            Prompt = text,
            StreamTokens = true
        };

        var response = await TogetherAI.GetCompletionResponseAsync(requestArgs, cancellationToken);

        var completion = new TogetherAICompletion(response);

        yield return new TogetherAITextStreamingResult(completion);
    }
}
