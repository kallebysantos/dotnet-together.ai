using System.Net.Http.Json;
using System.Runtime.CompilerServices;

namespace Together.AI;

public class TogetherAIClient : IDisposable
{
    private readonly HttpClient _httpClient = null!;

    public TogetherAIClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpResponseMessage> GetCompletionResponseAsync(
        TogetherAIRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    ) =>
        await _httpClient.PostAsJsonAsync(
            requestUri: "/inference",
            value: requestArgs,
            cancellationToken
        );

    public async IAsyncEnumerable<TogetherAIStreamResult> GetCompletionStreamAsync(
        TogetherAIRequestArgs requestArgs,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with { StreamTokens = true };

        using var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/inference",
            value: streamRequestArgs,
            cancellationToken
        );

        await foreach (var stream in response.ReadAsTogetherAIStreamAsync(cancellationToken))
        {
            yield return stream;
        }
    }

    public async Task<TogetherAIResult?> GetCompletionAsync(
        TogetherAIRequestArgs requestArgs,
        CancellationToken cancellationToken = default
    )
    {
        var streamRequestArgs = requestArgs with { StreamTokens = false };

        using var response = await _httpClient.PostAsJsonAsync(
            requestUri: "/inference",
            value: streamRequestArgs,
            cancellationToken
        );

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<TogetherAIResult>(cancellationToken);
    }

    public void Dispose() => _httpClient?.Dispose();
}
