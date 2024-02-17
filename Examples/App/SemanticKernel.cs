using System.Reflection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;

using Together.AI.SemanticKernel;

public static class SemanticKernelExample
{
    public static async Task RunExample(string apiKey, string? modelId = null)
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
}