using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

#if NET462
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace Together.AI
{
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
            using (var stream = await httpResponse.Content.ReadAsStreamAsync())
            {
                using (var reader = new StreamReader(stream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        if (!line.StartsWith("data:"))
                            continue;

                        var eventData = line.Substring("data:".Length).Trim();
                        if (eventData == null || eventData == "[DONE]")
                            break;

#if NET462
                        var result = JsonConvert.DeserializeObject<T>(eventData);
#else
                        var result = JsonSerializer.Deserialize<T>(eventData);
#endif

                        if (result != null)
                            yield return result;
                    }
                }
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

        /// <summary>
        /// Sends a POST request to the specified Uri containing the value serialized as JSON in the request body.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="requestUri"></param>
        /// <param name="value"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> PostAsJsonAsync(
            this HttpClient client,
            string requestUri,
            object value,
            CancellationToken cancellationToken = default
        )
        {
#if NET462
            var jsonContent = JsonConvert.SerializeObject(value);
#else
            var jsonContent = JsonSerializer.Serialize(value);
#endif

            var requestContent = new StringContent(
                content: jsonContent,
                encoding: Encoding.UTF8,
                mediaType: "application/json"
            );

            var request = new HttpRequestMessage(method: HttpMethod.Post, requestUri)
            {
                Content = requestContent
            };

            return await client.SendAsync(request, cancellationToken);
        }

        /// <summary>
        /// Reads the HTTP content and returns the value that results from deserializing the content as JSON in an asynchronous operation.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="cancellationToken"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static async Task<T> ReadFromJsonAsync<T>(
            this HttpContent content,
            CancellationToken cancellationToken = default
        )
        {
#if NET462
            var jsonContent = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(jsonContent);
#else

            using (var jsonStream = await content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<T>(
                    utf8Json: jsonStream,
                    cancellationToken: cancellationToken
                );
            }
#endif
        }
    }
}
