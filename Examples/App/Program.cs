using Together.AI;
// Uncomment it for execute a example that runs Raw together.ai client implementation
/*
await TogetherClientExample.RunTextCompletionExample(
    apiKey: "your-api-key-from-together",
    modelId: "togethercomputer/RedPajama-INCITE-7B-Instruct"
);
*/
var apikey = "your-api-key-from-together";

var client = new HttpClient();

client.SetupClient(apikey);

var together = new TogetherAIClient(client);

var chatCompletionArgs1 = new TogetherAIChatCompletionArgs
{
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
    Messages = new List<TogetherAIChatMessage>()
    {
        new () {
            Role = TogetherAIChatMessage.SystemRole,
            Content = "You are a helpful travel agent"
        },
        new () {
            Role = TogetherAIChatMessage.UserRole,
            Content = "Tell me about San Francisco"
        }
    },
};

var functionCallArgs1 = new TogetherAIChatCompletionArgs
{
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
    Messages = new List<TogetherAIChatMessage>()
    {
        new () {
            Role = TogetherAIChatMessage.SystemRole,
            Content = "You are a helpful assistant that can access external functions. The responses from these function calls will be appended to this dialogue. Please provide responses based on the information from these function calls."
        },
        new () {
            Role = TogetherAIChatMessage.UserRole,
            Content = "What is the current temperature of New York, San Francisco and Chicago?"
        }
    },
    Tools = new List<TogetherAITool>()
    {
        new TogetherAITool{
            Function = new ()
            {
                Name = "get_current_weather",
                Description ="Get the current weather in a given location",
                Parameters = new Dictionary<string, object>() {
                    {"type", "string"},
                    {"properties", new
                        {
                            location = new {
                                type= "string",
                                description = "The city and state, e.g. San Francisco, CA"
                            }
                        }},
                    {"unit", new
                        {
                            type = "string",
                            @enum = new string[] { "celsius", "fahrenheit" }
                        }}
                }
            }
        }
}
};

// var res = await together.GetChatCompletionResponseAsync(chatCompletionArgs1);
var res = await together.GetChatCompletionResponseAsync(functionCallArgs1);
var content = await res.Content.ReadAsStringAsync();
Console.WriteLine(content);

// await TogetherClientExample.RunTextCompletionExample(
//     apiKey: apikey,
//     modelId: "togethercomputer/RedPajama-INCITE-7B-Instruct"
// );
// Uncomment it for execute a example that runs Raw together.ai client implementation
/* await TogetherClientExample.RunTextEmbeddingExample(
    apiKey: "your-api-key-from-together",
    modelId: "togethercomputer/m2-bert-80M-8k-retrieval"
); */

// Uncomment it for execute a example that runs together.ai with Semantic Kernel
/* 
await SemanticKernelExample.RunExample(
    apiKey: "your-api-key-from-together",
    modelId: "togethercomputer/llama-2-7b"
); 
*/
