using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.CompilerServices;

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
        [EnumeratorCancellation] CancellationToken _ = default
    )
    {
        using var stream = await httpResponse.Content.ReadAsStreamAsync();

        using var reader = new StreamReader(stream);

        while (await reader.ReadLineAsync() is string line)
        {
            if (!line.StartsWith("data:"))
                continue;

            var eventData = line.Substring("data:".Length).Trim();
            if (eventData is null or "[DONE]")
                break;

            var result = JsonSerializer.Deserialize<T>(eventData);

            if (result is not null)
                yield return result;
        }
    }

    /// <summary>
    /// Extracts the data part of the SSE event and returns TogetherAIStreamResults
    /// </summary>
    /// <param name="httpResponse"></param>
    /// <param name="cancellationToken"></param>
    [Obsolete("This method uses the legacy 'inference' endpoint, please use the newer implementation.")]
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
