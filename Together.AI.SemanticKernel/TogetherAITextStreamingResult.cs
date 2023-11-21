using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Orchestration;

namespace Together.AI.SemanticKernel;

public class TogetherAITextStreamingResult : ITextStreamingResult, ITextResult
{
    private readonly TogetherAICompletion _completionData;

    public ModelResult ModelResult { get; }

    public TogetherAITextStreamingResult(TogetherAICompletion completionData)
    {
        _completionData = completionData;
        ModelResult = new ModelResult(completionData);
    }

    public Task<string> GetCompletionAsync(CancellationToken cancellationToken = default)
    {
        return _completionData.GetText(cancellationToken);
    }

    public IAsyncEnumerable<string> GetCompletionStreamingAsync(
        CancellationToken cancellationToken = default
    )
    {
        return _completionData.GetTextStreaming(cancellationToken);
    }
}
