using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Together.AI;

public static class TogetherAIClientExtensions
{
    /// <summary>
    /// Extracts the data part of the SSE event
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="cancellationToken"></param>
    public static async IAsyncEnumerable<T> ReadEventsAsync<T>(
        this HttpResponseMessage httpResponse,
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        using var stream = await httpResponse.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync(cancellationToken) is string line)
        {
            if (!line.StartsWith("data:"))
                continue;

            var eventData = line["data:".Length..].Trim();
            if (eventData is null or "[DONE]")
                break;

            var result = JsonSerializer.Deserialize<T>(
                json: eventData,
                options: new() { PropertyNameCaseInsensitive = true }
            );

            if (result is not null)
                yield return result;
        }
    }

    /// <summary>
    /// Extracts the data part of the SSE event and returns TogetherAIStreamResults
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="cancellationToken"></param>
    public static IAsyncEnumerable<TogetherAIStreamResult> ReadAsTogetherAIStreamAsync(
        this HttpResponseMessage httpResponse,
        CancellationToken cancellationToken = default
    )
    {
        httpResponse.EnsureSuccessStatusCode();

        return httpResponse.ReadEventsAsync<TogetherAIStreamResult>(cancellationToken);
    }

    /// <summary>
    /// Add default Together AI values for HttpClient
    /// </summary>
    /// <param name="client"></param>
    /// <param name="apiKey"></param>
    public static void SetupClient(this HttpClient client, string apiKey)
    {
        client.BaseAddress = new Uri("https://api.together.xyz");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            scheme: "Bearer",
            parameter: apiKey
        );
    }
}
