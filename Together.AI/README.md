<p align="center">
    <img src="https://github.com/kallebysantos/dotnet-together.ai/blob/master/Assets/together.svg" width="200" />
</p>


<div align="center">

[![Together Homepage](https://img.shields.io/badge/web-together.ai-blue?style=flat&label=https&colorB=0F6FFF)](https://www.together.ai)

[![Nuget](https://img.shields.io/nuget/v/Together.AI)](https://www.nuget.org/packages/Together.AI)
[![Nuget](https://img.shields.io/nuget/dt/Together.AI)](https://www.nuget.org/packages/Together.AI)
![GitHub License](https://img.shields.io/github/license/kallebysantos/dotnet-together.ai)
</div>

#### A unofficial .NET client for [Together's API platform](https://www.together.ai/).

## Getting started:

### Prerequisites

If you'd like to use the Together.AI .NET client library you'll need an API key from a developer account at [Together's API](https://api.together.xyz).

### Install the package

Install the client library for .NET with [NuGet](https://www.nuget.org/ ):

```bash
dotnet add package Together.AI
```

### Authenticate the client

In order to interact with Together's API, you'll need to create an instance of the `IHttpClient`
class that points to [Together's api endpoint URI](https://api.together.xyz) using a valid `Authorization` API key. 

The easiest way to do this is by using the `SetupClient` extension method.

```cs Snippet:MakeHttpClientWithTogetherAI
using Together.AI;

var client = new HttpClient();
client.SetupClient(apiKey: "your-api-key-from-together");
```

Then creates a instance of [`TogetherAIClient`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs) using the configured `HttpClient`.

```cs Snippet:CreateTogetherAIClient
var togetherAI = new TogetherAIClient(client);
```

## Start Querying

### Getting completion results

To simply get text completion results, you can use the [`GetCompletionAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L53) method.

```cs Snippet:GettingCompletionResults
using Together.AI;

// ...

// Setup Request Arguments
var togetherAIArgs = new TogetherAIRequestArgs()
{
    Model = "togethercomputer/RedPajama-INCITE-7B-Instruct",
    MaxTokens = 128,
    Prompt = "Alan Turing was "
};

var result = await togetherAI.GetCompletionAsync(togetherAIArgs);

// Print generated text
Console.WriteLine(result.Output.Choices[0].Text);
```

### Streaming tokens

To get tokens as the model generates, you can use the [`GetCompletionStreamAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L29) method.

```cs Snippet:StreamingTokens
using Together.AI;

// ...

var togetherAIArgs = new TogetherAIRequestArgs()
{
    Model = MODEL_ID,
    MaxTokens = 128,
    Prompt = "Alan Turing was "
};

await foreach (var streamResult in togetherAI.GetCompletionStreamAsync(togetherAIArgs))
{
    var token = streamResult.Choices[0].Text;

    // Print generated token
    Console.Write(token);
}
```

## Examples

### Looking for Grammar errors

```csharp
using Together.AI;

const string API_KEY = "your-api-key-from-together";
const string MODEL_ID = "Open-Orca/Mistral-7B-OpenOrca";
const string PROMPT_TEMPLATE =
    "<|im_start|> system\nDoes the text contain grammar errors? Answer with (Y/N)\n\n'{{$input}}'\n<|im_end|>\n<|im_start|> assistant\n";

var client = new HttpClient();
client.SetupClient(apiKey: API_KEY);

var togetherAI = new TogetherAIClient(client);

var promptInput = PROMPT_TEMPLATE.Replace(
    oldValue: "{{$input}}",
    newValue: "I mised the training session this morning"
);

var togetherAIArgs = new TogetherAIRequestArgs()
{
    Model = MODEL_ID,
    MaxTokens = 8,
    Prompt = promptInput
};

var result = await togetherAI.GetCompletionAsync(togetherAIArgs);

Console.WriteLine(result.Output.Choices[0].Text);

// Result: 'Y'
```