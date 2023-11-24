using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;

namespace Together.AI.SemanticKernel;

public class TogetherAITextStreamingResult(TogetherAICompletion completionData) : ITextStreamingResult, ITextResult
{
    public ModelResult ModelResult { get; } = new ModelResult(completionData);

    public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
    {
        return completionData.GetText(cancellationToken);
    }

    public IAsyncEnumerable<string> GetCompletionStreamingAsync(
        CancellationToken cancellationToken = default
    )
    {
        return completionData.GetTextStreaming(cancellationToken);
    }
}
