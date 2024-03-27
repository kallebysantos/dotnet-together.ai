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

To simply get text completion results, you can use the [`GetCompletionsAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L124) method.


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

var result = await togetherAI.GetCompletionsAsync(togetherAIArgs);

// Print generated text
Console.WriteLine(result?.Choices?.First().Text);
```

### Streaming tokens

To get tokens as the model generates, you can use the [`GetCompletionsStreamAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L78) method.

```cs Snippet:StreamingTokens
using Together.AI;

// ...

var togetherAIArgs = new TogetherAIRequestArgs()
{
    Model = MODEL_ID,
    MaxTokens = 128,
    Prompt = "Alan Turing was "
};

await foreach (var streamResult in togetherAI.GetCompletionsStreamAsync(togetherAIArgs))
{
    var token = streamResult.Choices.First().Text;

    // Print generated token
    Console.Write(token);
}
```

### Getting embeddings

To simply get text embedding results, you can use the [`GetEmbeddingsAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L167) method.

```cs Snippet:GettingEmbeddings
using Together.AI;

// ...

// Setup Request Arguments
var togetherAIArgs = new TogetherAIEmbeddingsRequestArgs()
{
    Model = MODEL_ID,
    Input = "Our solar system orbits the Milky Way galaxy at about 515,000 mph"
};

// Getting result
var result = await togetherAI.GetEmbeddingsAsync(togetherAIArgs);

// Print generated embeddings
foreach (var token in result?.Data?.First()?.Values ?? [])
{
    Console.Write(token);
}
```

### Getting chat completions

To simply get chat completion results, you can use the [`GetChatCompletionsAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L147) method.


```cs Snippet:GettingChatCompletion
using Together.AI;

// ...

// Creating a message history
var messages = new List<TogetherAIChatMessage>() {
    new TogetherAIChatSystemMessage("You are a helpful travel agent."),
    new TogetherAIChatUserMessage("Tell me about San Francisco.")
};

// Setup Request Arguments
var togetherAIArgs = new TogetherAIChatCompletionArgs
{
    Model = "mistralai/Mixtral-8x7B-Instruct-v0.1",
    Messages = messages,
    MaxTokens = 512,
    Stop = new string[]{
        "</s>",
        "[/INST]"
    },
};

var result = await togetherAI.GetChatCompletionsAsync(chatArgs);

if (result?.Choices?.FirstOrDefault()?.Message?.Content is string content)
{
    messages.Add(new TogetherAIChatAssistantMessage(content));
}

// Printing out the chat results
messages.ForEach(Console.WriteLine);

```

#### Function calling

The [`GetChatCompletionsAsync`](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Together.AI/TogetherAIClient.cs#L147)
method, can also be used to function calling. Like the following [Example](https://github.com/kallebysantos/dotnet-together.ai/blob/master/Examples/App/TogetherClient.cs#L144)

## Examples

### Looking for Grammar errors

```cs Snippet:GrammarErrorsExample
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

var result = await togetherAI.GetCompletionsAsync(togetherAIArgs);

Console.WriteLine(result?.Choices?.First().Text);

// Result: 'Y'
```

> More examples can be found in the [`Examples folder`](https://github.com/kallebysantos/dotnet-together.ai/tree/master/Examples)
