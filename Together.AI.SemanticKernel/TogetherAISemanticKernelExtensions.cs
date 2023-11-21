using System.Text.Json;
using Azure.Core.Extensions;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI;
using Microsoft.SemanticKernel.AI.TextCompletion;
using Microsoft.SemanticKernel.Http;

namespace Together.AI.SemanticKernel;

public static class TogetherAISemanticKernelExtensions
{
    public static KernelBuilder WithTogetherAIService(
        this KernelBuilder builder,
        string modelId,
        string apiKey,
        string? serviceId = null,
        bool setAsDefault = false,
        HttpClient? httpClient = null
    )
    {
        serviceId ??= nameof(TogetherAIService);

        builder.WithAIService(
            serviceId,
            (Func<ILoggerFactory, IDelegatingHandlerFactory, ITextCompletion>)(
                (ILoggerFactory loggerFactory, IDelegatingHandlerFactory httpHandlerFactory) =>
                {
                    if (httpClient is null)
                    {
                        var delegatingHandler = httpHandlerFactory.Create(loggerFactory);
                        delegatingHandler.InnerHandler = new HttpClientHandler();

                        httpClient = new HttpClient(
                            handler: delegatingHandler,
                            disposeHandler: false
                        );
                    }

                    httpClient.SetupClient(apiKey);

                    return new TogetherAIService(modelId, new TogetherAIClient(httpClient));
                }
            ),
            setAsDefault
        );
        return builder;
    }

    public static AIRequestSettings ToRequestSettings(this TogetherAIRequestArgs requestArgs)
    {
        var extensionDataJson = JsonSerializer.Serialize(requestArgs);

        var extensionData =
            JsonSerializer.Deserialize<Dictionary<string, object>>(extensionDataJson)
            ?? throw new ArgumentException(
                message: $"Cannot convert to {nameof(AIRequestSettings)}",
                paramName: nameof(requestArgs)
            );

        return new AIRequestSettings { ModelId = requestArgs.Model, ExtensionData = extensionData };
    }

    public static TogetherAIRequestArgs ToTogetherArgs(this AIRequestSettings requestSettings)
    {
        var extensionDataJson = JsonSerializer.Serialize(requestSettings.ExtensionData);

        var togetherAIArgs =
            JsonSerializer.Deserialize<TogetherAIRequestArgs>(extensionDataJson)
            ?? throw new ArgumentException(
                message: $"Cannot convert to {nameof(TogetherAIRequestArgs)}",
                paramName: nameof(requestSettings)
            );

        return togetherAIArgs with
        {
            Model = requestSettings.ModelId
        };
    }
}
