using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace Together.AI.SemanticKernel;

public class TogetherAIChatCompletionService(TogetherAIClient TogetherAI, string? ModelId = null) : IChatCompletionService
{
    public IReadOnlyDictionary<string, object?> Attributes => new Dictionary<string, object?>();

    public async Task<IReadOnlyList<ChatMessageContent>> GetChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default
    )
    {
        var requestArgs = PrepareArgs(chatHistory, executionSettings);

        var completion = await TogetherAI.GetChatCompletionsAsync(
                requestArgs: requestArgs,
                cancellationToken
        );

        if (completion is null || !completion.Choices.Any())
            throw new KernelException("Chat completions not found");

        // If we don't want to attempt to invoke any functions, just return the result.
        // Or if we are auto-invoking but we somehow end up with other than 1 choice even though only 1 was requested, similarly bail.
        if (
            kernel is null ||
            requestArgs.ToolChoice is not TogetherAIAutoToolChoice ||
            completion.Choices?.Length != 1
        )
            return completion.Choices
                .Where(chatChoice => chatChoice.Message is not null)
                .Select(chatChoice => chatChoice.Message!.ToKernelMessage())
                .ToList();

        // Get our single result and extract the function call information. If this isn't a function call, or if it is
        // but we're unable to find the function or extract the relevant information, just return the single result.
        // Note that we don't check the FinishReason and instead check whether there are any tool calls, as the service
        // may return a FinishReason of "stop" even if there are tool calls to be made, in particular if a required tool
        // is specified.
        var result = completion.Choices.First();

        if (
            result.Message?.ToolCalls is not TogetherAIToolCall[] toolCalls ||
            !result.Message.ToolCalls.Any()
        )
        {
            var parsedResult = result.Message?.ToKernelMessage();
            return parsedResult is not null ? new[] { parsedResult } : [];
        }

        throw new NotImplementedException();

        // Todo function calling
    }

    public IAsyncEnumerable<StreamingChatMessageContent> GetStreamingChatMessageContentsAsync(
            ChatHistory chatHistory,
            PromptExecutionSettings? executionSettings = null,
            Kernel? kernel = null,
            CancellationToken cancellationToken = default
    )
    {
        throw new System.NotImplementedException();
    }

    protected TogetherAIChatCompletionArgs PrepareArgs(ChatHistory chatHistory, PromptExecutionSettings? executionSettings = null)
    {
        var modelArgs = executionSettings?.ToTogetherModelArgs() ?? new TogetherAIModelArgs();

        return new TogetherAIChatCompletionArgs(modelArgs) with
        {
            Messages = chatHistory.Select(msg => msg.ToTogetherChatMessage()),
            Model = modelArgs.Model ?? ModelId
                ?? throw new ArgumentNullException(nameof(ModelId),
                    $"No model supplied: {nameof(TogetherAITextGenerationService)} requires a valid model."
                ),
        };
    }


}
