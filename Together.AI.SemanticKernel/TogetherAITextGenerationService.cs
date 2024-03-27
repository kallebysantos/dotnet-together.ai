using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.SemanticKernel;

namespace Together.AI.SemanticKernel;

public class TogetherAITextGenerationService(TogetherAIClient TogetherAI, string? ModelId = null) : ITextGenerationService
{
    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public async IAsyncEnumerable<StreamingTextContent> GetStreamingTextContentsAsync(
        string prompt,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var completionStream = TogetherAI.GetCompletionsStreamAsync(
            requestArgs: PrepareArgs(prompt, executionSettings),
            cancellationToken
        );

        await foreach (var completion in completionStream)
        {
            var textResult = completion.Choices?.First()?.Text ?? string.Empty;

            yield return new(textResult);
        }
    }

    public async Task<IReadOnlyList<TextContent>> GetTextContentsAsync(
        string prompt,
        PromptExecutionSettings? executionSettings = null,
        Kernel? kernel = null,
        CancellationToken cancellationToken = default
    )
    {
        var completion = await TogetherAI.GetCompletionsAsync(
            requestArgs: PrepareArgs(prompt, executionSettings),
            cancellationToken
        );

        var textResult = completion?.Choices.First()?.Text ?? string.Empty;

        return [new(textResult)];
    }

    protected TogetherAICompletionArgs PrepareArgs(string prompt, PromptExecutionSettings? executionSettings = null)
    {
        var modelArgs = executionSettings?.ToTogetherModelArgs() ?? new TogetherAIModelArgs();

        return (TogetherAICompletionArgs)modelArgs with
        {
            Prompt = prompt,
            Model = modelArgs.Model ?? ModelId
                ?? throw new ArgumentNullException(nameof(ModelId),
                    $"No model supplied: {nameof(TogetherAITextGenerationService)} requires a valid model."
                ),
        };
    }
}
