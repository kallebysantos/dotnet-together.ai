using Together.AI;

public static class TogetherClientExample
{
    public static async Task RunExample(string apiKey, string modelId)
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
}