using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.TextGeneration;
using Microsoft.SemanticKernel.ChatCompletion;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Together.AI.SemanticKernel;

public static class TogetherAISemanticKernelExtensions
{
    public static IKernelBuilder AddTogetherAITextGeneration(
        this IKernelBuilder builder,
        string? modelId = null,
        TogetherAIClient? togetherAIClient = null,
        string? serviceId = null
    )
    {
        serviceId ??= nameof(TogetherAITextGenerationService);

        builder.Services.AddKeyedSingleton<ITextGenerationService, TogetherAITextGenerationService>(
            serviceKey: nameof(TogetherAITextGenerationService),
            implementationFactory: (services, _) =>
            {
                togetherAIClient ??= services.GetRequiredService<TogetherAIClient>();
                return new TogetherAITextGenerationService(
                    TogetherAI: togetherAIClient,
                    ModelId: modelId
                );
            });

        return builder;
    }

    public static IKernelBuilder AddTogetherAIChatCompletion(
        this IKernelBuilder builder,
        string? modelId = null,
        TogetherAIClient? togetherAIClient = null,
        string? serviceId = null
    )
    {
        serviceId ??= nameof(TogetherAIChatCompletionService);

        builder.Services.AddKeyedSingleton<IChatCompletionService, TogetherAIChatCompletionService>(
            serviceKey: nameof(TogetherAITextGenerationService),
            implementationFactory: (services, _) =>
            {
                togetherAIClient ??= services.GetRequiredService<TogetherAIClient>();
                return new TogetherAIChatCompletionService(
                    TogetherAI: togetherAIClient,
                    ModelId: modelId
                );
            });

        return builder;
    }

    public static IKernelBuilder AddTogetherAITextGeneration(
        this IKernelBuilder builder,
        string apiKey,
        string? modelId = null,
        string? serviceId = null
    )
    {
        builder.Services.AddHttpClient<TogetherAIClient>(c => c.SetupClient(apiKey));

        return builder.AddTogetherAITextGeneration(
            modelId: modelId,
            serviceId: serviceId
        );
    }

    public static IKernelBuilder AddTogetherAIChatCompletion(
        this IKernelBuilder builder,
        string apiKey,
        string? modelId = null,
        string? serviceId = null
    )
    {
        builder.Services.AddHttpClient<TogetherAIClient>(c => c.SetupClient(apiKey));

        return builder.AddTogetherAIChatCompletion(
            modelId: modelId,
            serviceId: serviceId
        );
    }

    public static IKernelBuilder AddTogetherAITextGeneration(
        this IKernelBuilder builder,
        HttpClient httpClient,
        string? modelId = null,
        string? serviceId = null
    )
        => builder.AddTogetherAITextGeneration(
            modelId: modelId,
            togetherAIClient: new TogetherAIClient(httpClient),
            serviceId: serviceId
        );

    public static IKernelBuilder AddTogetherAIChatCompletion(
        this IKernelBuilder builder,
        HttpClient httpClient,
        string? modelId = null,
        string? serviceId = null
    )
        => builder.AddTogetherAIChatCompletion(
            modelId: modelId,
            togetherAIClient: new TogetherAIClient(httpClient),
            serviceId: serviceId
        );

    public static PromptExecutionSettings ToPromptExecutionSettings<T>(T settings)
        where T : TogetherAIModelArgs
    {
        var extensionDataJson = JsonSerializer.Serialize(settings);

        var extensionData =
            JsonSerializer.Deserialize<Dictionary<string, object>>(extensionDataJson)
            ?? throw new ArgumentException(
                message: $"Cannot convert to {nameof(PromptExecutionSettings)}",
                paramName: nameof(settings)
            );

        return new PromptExecutionSettings
        {
            ModelId = settings.Model,
            ExtensionData = extensionData
        };
    }

    public static TogetherAIModelArgs ToTogetherModelArgs(this PromptExecutionSettings promptSettings)
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var extensionDataJson = JsonSerializer.Serialize(promptSettings, options: jsonOptions);

        var togetherAIArgs =
            JsonSerializer.Deserialize<TogetherAIModelArgs>(extensionDataJson, options: jsonOptions)
            ?? throw new ArgumentException(
                message: $"Cannot convert to {nameof(TogetherAIModelArgs)}",
                paramName: nameof(PromptExecutionSettings)
            );

        return togetherAIArgs with
        {
            Model = promptSettings.ModelId
        };
    }

    public static ChatMessageContent ToKernelMessage(this TogetherAIChatMessage message)
        => message switch
        {
            TogetherAIChatSystemMessage msg => new ChatMessageContent(AuthorRole.System, msg.Content),
            TogetherAIChatUserMessage msg => new ChatMessageContent(AuthorRole.User, msg.Content),
            TogetherAIChatAssistantMessage msg => new ChatMessageContent(AuthorRole.Assistant, msg.Content),
            TogetherAIChatToolCallMessage msg => new ChatMessageContent(
                    role: AuthorRole.Tool,
                    content: msg.Content,
                    metadata: new Dictionary<string, object?>()
                    {
                        { nameof(TogetherAIChatToolCallMessage.Id), msg.Id },
                        { nameof(TogetherAIChatToolCallMessage.Name), msg.Name},
                    }
                ),
            _ => new ChatMessageContent(new AuthorRole(), message.Content)
        };

    public static ChatMessageContent ToKernelMessage(this TogetherAIChatMessageResult messageResult)
        => messageResult switch
        {
            var msg when msg.Role == TogetherAIChatSystemMessage.SystemRole => new TogetherAIChatSystemMessage(msg.Content ?? string.Empty).ToKernelMessage(),
            var msg when msg.Role == TogetherAIChatUserMessage.UserRole => new TogetherAIChatUserMessage(msg.Content ?? string.Empty).ToKernelMessage(),
            var msg when msg.Role == TogetherAIChatAssistantMessage.AssistantRole => new TogetherAIChatAssistantMessage(msg.Content ?? string.Empty).ToKernelMessage(),
            var msg when msg.Role == TogetherAIChatToolCallMessage.ToolRole => new ChatMessageContent(
                    role: AuthorRole.Tool,
                    content: msg.Content,
                    metadata: new Dictionary<string, object?>()
                    {
                        { nameof(TogetherAIChatMessageResult.ToolCalls ), msg.ToolCalls },
                    }
                ),
            _ => throw new InvalidCastException("Could not convert from given message")
        };

    public static TogetherAIChatMessage ToTogetherChatMessage(this ChatMessageContent message)
     => message switch
     {
         var msg when msg.Role == AuthorRole.System => new TogetherAIChatSystemMessage(msg.Content ?? string.Empty),
         var msg when msg.Role == AuthorRole.User => new TogetherAIChatUserMessage(msg.Content ?? string.Empty),
         var msg when msg.Role == AuthorRole.Assistant => new TogetherAIChatAssistantMessage(msg.Content ?? string.Empty),
         var msg when msg.Role == AuthorRole.Tool => new TogetherAIChatToolCallMessage(
                    Id: msg.Metadata?.FirstOrDefault(arg => arg.Key == nameof(TogetherAIChatToolCallMessage.Id)).Value?.ToString() ?? string.Empty,
                    FunctionName: msg.Metadata?.FirstOrDefault(arg => arg.Key == nameof(TogetherAIChatToolCallMessage.Name)).Value?.ToString() ?? string.Empty,
                    FunctionResponse: msg.Content ?? string.Empty
                 ),
         _ => throw new InvalidCastException("Could not convert from given message")
     };

    public static TogetherAIFunctionTool ToTogetherFunction(this KernelFunctionMetadata kernelFunction)
    {
        var function = new TogetherAIFunctionTool
        {
            Name = $"{kernelFunction.PluginName}::{kernelFunction.Name}",
            Description = kernelFunction.Description,
        };

        if (kernelFunction.Parameters.Any())
        {
            var properties = kernelFunction.Parameters.ToDictionary(
                keySelector: metadata => metadata.Name,
                elementSelector: metadata => new
                {
                    type = metadata.ParameterType?.Name.ToLower(),
                    description = metadata.Description,
                }
            );

            var requiredProperties = kernelFunction.Parameters
                .Where(param => param.IsRequired)
                .Select(param => param.Name);

            function = function with
            {
                Parameters = new Dictionary<string, object>()
                {
                    {"type", "object"},
                    {"properties", properties},
                    {"required", requiredProperties},
                }
            };
        }

        return function;
    }
}
