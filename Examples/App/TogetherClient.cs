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

    public static async Task RunFunctionCallExample(string apiKey, string modelId)
    {
        // The function to be called, could also be a instance method or anything else.
        Func<string, dynamic> GetCurrentWeather =
            (string location) => location.ToLower() switch
            {
                "chicago" => new { location = "Chicago", temperature = 13 },
                "san francisco" => new { location = "San Francisco", temperature = 35 },
                "new york" => new { location = "New York", temperature = 11 },
                _ => new { location, temperature = "unknown" },
            };

        // Creating a tool metadata object
        var getCurrentWeatherTool = new TogetherAITool
        {
            Function = new()
            {
                Name = nameof(GetCurrentWeather),
                Description = "Get the current weather in a given location",
                Parameters = new Dictionary<string, object>() {
                    {"type", "string"},
                    {"properties", new
                        {
                            location = new {
                                type= "string",
                                description = "The location to get weather. Must be a single location name"
                            }
                        }},
                }
            }
        };

        var client = new HttpClient();
        client.SetupClient(apiKey);

        var togetherAI = new TogetherAIClient(client);

        var messages = new List<TogetherAIChatMessage>() {
            new TogetherAIChatSystemMessage(
                    Content: "You are a helpful assistant that can access external functions. The responses from these function calls will be appended to this dialogue. Please provide responses based on the information from these function calls."),
                new TogetherAIChatUserMessage(
                        Content: "What is the current temperature of the following locations: New York, Chicago and San Francisco?")
        };

        // Function calling arguments
        var functionArgs = new TogetherAIChatCompletionArgs
        {
            // At this moment, only 3 models can do Function calling:
            // - mistralai/Mixtral-8x7B-Instruct-v0.1
            // - mistralai/Mistral-7B-Instruct-v0.1
            // - togethercomputer/CodeLlama-34b-Instruct
            Model = "mistralai/Mixtral-8x7B-Instruct-v0.1",
            Stop = new string[]{
                "</s>",
                "[/INST]"
            },
            MaxTokens = 512,
            Temperature = 0.7,
            TopP = 0.7,
            TopK = 50,
            RepetitionPenalty = 1,
            Messages = messages,
            Tools = new List<TogetherAITool>()
            {
                // Adding the function metadata to available tools
                // So the LLM will give us all necessary invokes
                getCurrentWeatherTool
            }
        };

        // First call the LLM to receive all necessary function calls
        var functionCallResult = await togetherAI.GetChatCompletionsAsync(functionArgs);
        if (
                functionCallResult?.Choices?.FirstOrDefault()
                is not TogetherAIChatCompletionChoice functionCallChoice
           )
        {
            Console.WriteLine("No choices returned. Exiting...");
            return;
        }

        if (functionCallChoice.FinishReason != TogetherAIChatCompletionChoice.ToolCallFinishReason)
        {
            Console.WriteLine("No function calls returned. Exiting...");
            return;
        }

        // Then we iterate over all `tool calls` and invoke the necessary functions
        foreach (var toolCall in functionCallChoice.Message?.ToolCalls ?? [])
        {
            if (toolCall.Function?.Name == nameof(GetCurrentWeather))
            {
                var location = toolCall.Function.GetArguments?
                    .FirstOrDefault(arg => arg.Key == "location").Value.ToString()
                    ?? string.Empty;

                // Adding the tool call result in the message history
                messages.Add(TogetherAIChatToolCallMessage.FromToolCall(
                    toolCall: toolCall,
                    functionResponse: GetCurrentWeather(location)
                ));
            }
        }

        // Now, we call the model again but providing the tool call messages as context.
        // So that the LLM can provide enriched response

        // Setup Enriched request Arguments
        var enrichedArgs = new TogetherAIChatCompletionArgs
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
        var result = await togetherAI.GetChatCompletionsAsync(enrichedArgs);

        if (result?.Choices?.FirstOrDefault()?.Message?.Content is string content)
        {
            messages.Add(new TogetherAIChatAssistantMessage(content));

            // Printing out the chat results
            messages.Where(msg => msg.Role != TogetherAIChatToolCallMessage.ToolRole)
                    .Select(msg => $"{msg.Role}: {msg.Content}")
                    .ToList()
                    .ForEach(Console.WriteLine);
        }
        else
        {
            Console.WriteLine("Error: Assistant, didn't respond");
        }
    }
}
