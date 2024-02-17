using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel.TextGeneration;

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

    public static TogetherAIRequestArgs ToTogetherArgs(this PromptExecutionSettings promptSettings)
    {
        var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var extensionDataJson = JsonSerializer.Serialize(promptSettings, options: jsonOptions);

        var togetherAIArgs =
            JsonSerializer.Deserialize<TogetherAIRequestArgs>(extensionDataJson, options: jsonOptions)
            ?? throw new ArgumentException(
                message: $"Cannot convert to {nameof(TogetherAIRequestArgs)}",
                paramName: nameof(PromptExecutionSettings)
            );

        return togetherAIArgs with
        {
            Model = promptSettings.ModelId
        };
    }
}
