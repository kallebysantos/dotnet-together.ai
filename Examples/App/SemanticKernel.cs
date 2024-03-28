using System.ComponentModel;
using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Together.AI;
using Together.AI.SemanticKernel;

public static class SemanticKernelExample
{
    public class WeatherPlugin
    {
        [KernelFunction]
        [Description("Get the current weather in a given location.'")]
        public dynamic GetCurrentWeather([Description("The city and state, e.g. San Francisco, CA")] string location) => location.ToLower() switch
        {
            "chicago" => new { location = "Chicago", temperature = 13 },
            "san francisco" => new { location = "San Francisco", temperature = 35 },
            "new york" => new { location = "New York", temperature = 11 },
            _ => new { location, temperature = "unknown" },
        };
    }

    public static async Task RunLoadingExternalPromptExample(string apiKey, string? modelId = null)
    {

        var kernel = Kernel.CreateBuilder()
            .AddTogetherAITextGeneration(apiKey, modelId)
            .Build();

        // Load prompt from YAML
        var assembly = Assembly.GetExecutingAssembly();
        var fileNames = assembly.GetManifestResourceNames();
        var generateHistoryFile = fileNames.First(f => f.Contains("GenerateHistory"));
        using StreamReader reader = new(assembly.GetManifestResourceStream(generateHistoryFile)!);

        var generateHistory = kernel.CreateFunctionFromPromptYaml(
            reader.ReadToEnd(),
            promptTemplateFactory: new HandlebarsPromptTemplateFactory()
        );

        var completionStream = kernel.InvokeStreamingAsync(generateHistory, arguments: new()
        {
            { "topic", "A dog who loves play with his ball" }
        });

        await foreach (var completion in completionStream)
        {
            Console.Write(completion);
        }
    }

    public static async Task RunChatExample(string apiKey, string? modelId = null)
    {
        var kernelBuilder = Kernel.CreateBuilder()
            .AddTogetherAIChatCompletion(apiKey, modelId);

        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        Console.Write("User: ");
        string? userInput;
        while ((userInput = Console.ReadLine()) != null)
        {
            history.AddUserMessage(userInput);

            var result = await chat.GetChatMessageContentAsync(history);

            Console.WriteLine("Assistant: " + result);

            history.AddMessage(result.Role, result.Content ?? string.Empty);

            Console.Write("User: ");
        }
    }


    public static async Task RunFunctionCall(string apiKey, string? modelId = null)
    {
        var kernelBuilder = Kernel.CreateBuilder()
            .AddTogetherAIChatCompletion(apiKey, modelId);

        kernelBuilder.Plugins.AddFromType<WeatherPlugin>();

        var kernel = kernelBuilder.Build();

        var chat = kernel.GetRequiredService<IChatCompletionService>();

        var history = new ChatHistory();

        history.AddSystemMessage("You are a helpful assistant that can access external functions. The responses from these function calls will be appended to this dialogue. Please provide responses based on the information from these function calls.");
        history.AddUserMessage("What is the current temperature of New York, San Francisco and Chicago?.");

        var executionSettings = new TogetherAIChatCompletionArgs
        {
            ToolChoice = new TogetherAIAutoToolChoice()
        };

        var result = await chat.GetChatMessageContentAsync(
            history,
            TogetherAISemanticKernelExtensions.ToPromptExecutionSettings(executionSettings),
            kernel
        );

        history.Add(result);

        foreach (var message in history)
        {
            Console.WriteLine($"{message.Role}: {message}");
        }
    }
}
