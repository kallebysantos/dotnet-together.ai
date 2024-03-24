using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Together.AI;

public class TogetherAIClient(HttpClient httpClient) : IDisposable
{
    private readonly HttpClient _httpClient = httpClient;

    [Obsolete("This method uses the legacy 'inference' endpoint, please use the newer implementation.")]
    public async Task<HttpResponseMessage> GetCompletionResponseAsync(
        TogetherAIRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    ) =>
        await _httpClient.PostAsJsonAsync(
            requestUri: "/inference",
            value: requestArgs,
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

        using var response = await GetCompletionResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );

        await foreach (var stream in response.ReadAsTogetherAIStreamAsync(cancellationToken))
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

        using var response = await GetCompletionResponseAsync(
            requestArgs: streamRequestArgs,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAIResult>(cancellationToken);
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

