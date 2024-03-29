﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Together.AI;

public class TogetherAIClient(HttpClient httpClient) : IDisposable
{
    private readonly HttpClient _httpClient = httpClient;

    public JsonSerializerOptions GetJsonSerializerOptions => new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    [Obsolete("This method uses the legacy 'inference' endpoint, please use the newer implementation.")]
    public async Task<HttpResponseMessage> GetInferenceResponseAsync(
        TogetherAIRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    ) =>
        await _httpClient.PostAsJsonAsync(
            requestUri: "/inference",
            value: requestArgs,
            options: GetJsonSerializerOptions,
            cancellationToken
        );

    public async Task<HttpResponseMessage> GetCompletionResponseAsync(
        TogetherAICompletionArgs requestArgs,
        CancellationToken cancellationToken = default
    ) =>
        await _httpClient.PostAsJsonAsync(
            requestUri: "/v1/completions",
            value: requestArgs,
            options: GetJsonSerializerOptions,
            cancellationToken
        );

    public async Task<HttpResponseMessage> GetChatCompletionResponseAsync(
        TogetherAIChatCompletionArgs requestArgs,
        CancellationToken cancellationToken = default
    ) =>
        await _httpClient.PostAsJsonAsync(
            requestUri: "/v1/chat/completions",
            value: requestArgs,
            options: GetJsonSerializerOptions,
            cancellationToken
        );

    [Obsolete("This method uses the legacy 'inference' endpoint, please use the newer implementation.")]
    public async IAsyncEnumerable<TogetherAIStreamResult> GetCompletionStreamAsync(
        TogetherAIRequestArgs requestArgs,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with
        {
            StreamTokens = true
        };

        using var response = await GetInferenceResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );

        await foreach (var stream in response.ReadAsTogetherAIStreamAsync(cancellationToken))
        {
            yield return stream;
        }
    }

    public async IAsyncEnumerable<TogetherAICompletionResult> GetCompletionsStreamAsync(
        TogetherAICompletionArgs requestArgs,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with
        {
            Stream = true
        };

        using var response = await GetCompletionResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        await foreach (var stream in response.ReadEventsAsync<TogetherAICompletionResult>(cancellationToken))
        {
            yield return stream;
        }
    }

    [Obsolete("This method uses the legacy 'inference' endpoint, please use the newer implementation.")]
    public async Task<TogetherAIResult?> GetCompletionAsync(
        TogetherAIRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with
        {
            StreamTokens = false
        };

        using var response = await GetInferenceResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAIResult>(cancellationToken);
    }

    /// <summary>
    /// Method for language, code, and image models on Together AI
    /// </summary>
    public async Task<TogetherAICompletionResult?> GetCompletionsAsync(
        TogetherAICompletionArgs requestArgs,
        CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with
        {
            Stream = false
        };

        using var response = await GetCompletionResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAICompletionResult>(cancellationToken);
    }

    /// <summary>
    /// Method for chat and moderation models on Together AI
    /// </summary>
    public async Task<TogetherAIChatCompletionResult?> GetChatCompletionsAsync(
        TogetherAIChatCompletionArgs requestArgs,
        CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with
        {
            Stream = false
        };

        using var response = await GetChatCompletionResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAIChatCompletionResult>(cancellationToken);
    }

    public async Task<TogetherAIEmbeddingsResult?> GetEmbeddingsAsync(
        TogetherAIEmbeddingsRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    )
    {
        var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/v1/embeddings",
            value: requestArgs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAIEmbeddingsResult>(cancellationToken);
    }

    public void Dispose() => _httpClient?.Dispose();
}

