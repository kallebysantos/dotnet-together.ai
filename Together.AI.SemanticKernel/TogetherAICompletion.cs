using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Together.AI.SemanticKernel;

public class TogetherAICompletion(HttpResponseMessage HttpResponse)
{
    public string Completion { get; set; } = string.Empty;

    public IEnumerable<string> Tokens { get; set; } = Enumerable.Empty<string>();

    public async Task<string> GetText(CancellationToken cancellationToken = default)
    {
        if (Completion == string.Empty)
        {
            HttpResponse.EnsureSuccessStatusCode();

            var result = await HttpResponse.Content.ReadFromJsonAsync<TogetherAIResult>(
                cancellationToken
            );

            if (result?.Output?.Choices?.First().Text is string completionResult)

                Completion = completionResult;
        }

        return Completion;
    }

    public async IAsyncEnumerable<string> GetTextStreaming(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        await foreach (
            var streamResult in HttpResponse.ReadAsTogetherAIStreamAsync(cancellationToken)
        )
        {
            if (streamResult.Choices?.First().Text is string token)
            {
                Completion = string.Concat(Tokens.Append(token));

                yield return token;
            }
        }
    }
}
