[![Together Homepage](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Assets/together.png?raw=true)](https://www.together.ai)

#### A unofficial .NET Semantic Kernel integration for [Together's API platform](https://www.together.ai/).

## Getting started:

### Prerequisites

If you'd like to use the Together.AI .NET client library you'll need an API key from a developer account at [Together's API](https://api.together.xyz).

### Install the package

Install the client library for .NET with [NuGet](https://www.nuget.org/ ):

```bash
dotnet add package Together.AI.SemanticKernel
```

### Setup TogetherAI Service

To utilize Together's API seamlessly, you'll first need to register the [`TogetherAITextGenerationService`]()
in `IKernelBuilder`

The easiest way to do this is by using the [`AddTogetherAITextGeneration`]() extension method.

```cs Snippet:SetupTogetherAIService
using Microsoft.SemanticKernel;
using Together.AI.SemanticKernel;

string apiKey = "your-api-key-from-together";

// You can omit modelId if you pretend to supply it from SK function parameters.
string? modelId = "togethercomputer/RedPajama-INCITE-7B-Instruct";

var kernel = Kernel.CreateBuilder()
    .AddTogetherAITextGeneration(apiKey, modelId)
    .Build();
```

With this setup, you are now equipped to seamlessly continue using SK and its built-in features.

> The [`AddTogetherAITextGeneration`]() method supports additional overrides for more precise configuration.

## Examples

### Looking for Grammar errors

```csharp
using Microsoft.SemanticKernel;
using Together.AI.SemanticKernel;

var kernel = Kernel.CreateBuilder()
    .AddTogetherAITextGeneration(
        apiKey: "your-api-key-from-together",
        modelId: "Open-Orca/Mistral-7B-OpenOrca")
    .Build();

const string promptTemplate =
    "<|im_start|> system\nDoes the text contain grammar errors? Answer with (Y/N)\n\n'{{$input}}'\n<|im_end|>\n<|im_start|> assistant\n";

var grammarValidation = kernel.CreateFunctionFromPrompt(promptTemplate);

var result = await kernel.InvokeAsync(grammarValidation, arguments: new()
{
    {"input", "I mised the training session this morning"}
});

Console.WriteLine(result);

// Result: 'Y'
```