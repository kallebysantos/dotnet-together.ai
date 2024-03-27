using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using Together.AI.SemanticKernel;

public static class SemanticKernelExample
{
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
}
