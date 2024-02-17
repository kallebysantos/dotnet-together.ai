using Together.AI;

public static class TogetherClientExample
{
    public static async Task RunTextCompletionExample(string apiKey, string modelId)
    {
        var client = new HttpClient();
        client.SetupClient(apiKey);

        var togetherAI = new TogetherAIClient(client);

        // Setup Request Arguments
        var togetherAIArgs = new TogetherAIRequestArgs()
        {
            Model = modelId,
            MaxTokens = 128,
            Prompt = "Alan Turing was "
        };

        // Getting completion result
        var result = await togetherAI.GetCompletionAsync(togetherAIArgs);

        // Print generated text
        Console.WriteLine(result?.Output?.Choices?.First().Text);

        // Getting completion as stream
        await foreach (var streamResult in togetherAI.GetCompletionStreamAsync(togetherAIArgs))
        {
            var token = streamResult?.Choices?.First().Text;

            // Print generated token
            Console.Write(token);
        }
    }

    public static async Task RunTextEmbeddingExample(string apiKey, string modelId)
    {
        var client = new HttpClient();
        client.SetupClient(apiKey);

        var togetherAI = new TogetherAIClient(client);

        // Setup Request Arguments
        var togetherAIArgs = new TogetherAIEmbeddingsRequestArgs()
        {
            Model = modelId,
            Input = "Our solar system orbits the Milky Way galaxy at about 515,000 mph"
        };

        // Getting result
        var result = await togetherAI.GetEmbeddingsAsync(togetherAIArgs);

        // Print generated embeddings
        foreach (var token in result?.Data?.First()?.Values ?? [])
        {
            Console.Write(token);
        }
    }
}