using Together.AI;

public static class TogetherClientExample
{
    [Obsolete("Uses the old 'Inference' endpoint")]
    public static async Task RunTextInferenceExample(string apiKey, string modelId)
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

    public static async Task RunTextCompletionExample(string apiKey, string modelId)
    {
        var client = new HttpClient();
        client.SetupClient(apiKey);

        var togetherAI = new TogetherAIClient(client);

        // Setup Request Arguments
        var togetherAIArgs = new TogetherAICompletionArgs()
        {
            Model = modelId,
            MaxTokens = 128,
            Prompt = "Alan Turing was "
        };

        // Getting completion result
        var result = await togetherAI.GetCompletionsAsync(togetherAIArgs);

        // Print generated text
        Console.WriteLine(result?.Choices?.First().Text);

        // Getting completion as stream
        await foreach (var streamResult in togetherAI.GetCompletionsStreamAsync(togetherAIArgs))
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

    public static async Task RunChatCompletionExample(string apiKey, string modelId)
    {
        var client = new HttpClient();
        client.SetupClient(apiKey);

        var togetherAI = new TogetherAIClient(client);

        var messages = new List<TogetherAIChatMessage>() {
            new TogetherAIChatSystemMessage("You are a helpful AI assistant.")
        };

        Console.WriteLine("AI CHAT - write 'exit' to finish.");
        Console.WriteLine("---------------------------------");

        Console.Write("User: ");
        string? userInput;

        while (
            (userInput = Console.ReadLine()) is not null &&
            !userInput.Contains("exit")
        )
        {
            messages.Add(new TogetherAIChatUserMessage(userInput));

            // Setup Request Arguments
            var chatArgs = new TogetherAIChatCompletionArgs
            {
                Model = modelId,
                Stop = new string[]{
                    "</s>",
                    "[/INST]"
                },
                MaxTokens = 512,
                Messages = messages,
            };

            // Getting result
            var result = await togetherAI.GetChatCompletionsAsync(chatArgs);

            if (result?.Choices?.FirstOrDefault()?.Message?.Content is string content)
            {
                messages.Add(new TogetherAIChatAssistantMessage(content));
                Console.WriteLine($"Assistant: {content}");
            }
            else
            {
                Console.WriteLine("Error");
            }

            Console.Write("User: ");
        }
    }
}
