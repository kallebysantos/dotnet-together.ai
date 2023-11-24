using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Together.AI;

namespace App.Legacy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var client = new HttpClient();
            client.SetupClient(apiKey: "");

            var togetherAI = new TogetherAIClient(client);

            // Setup Request Arguments
            var togetherAIArgs = new TogetherAIRequestArgs()
            {
                Model = "togethercomputer/RedPajama-INCITE-7B-Instruct",
                MaxTokens = 128,
                Prompt = "Alan Turing was "
            };

            // Getting Completion

            var completionResult = await togetherAI.GetCompletionAsync(togetherAIArgs);

            Console.WriteLine(completionResult.Output.Choices[0].Text);


            // Getting Completion as Stream - Requires C# 8 or greater

            var completionStreamResult = new StringBuilder();

            await foreach (var streamResult in togetherAI.GetCompletionStreamAsync(togetherAIArgs))
            {
                var token = streamResult.Choices[0].Text;

                completionStreamResult.Append(token);
            }

            Console.WriteLine(completionStreamResult.ToString());
        }
    }
}
