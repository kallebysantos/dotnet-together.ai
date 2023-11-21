// See https://aka.ms/new-console-template for more information
using Microsoft.SemanticKernel;
using Together.AI.SemanticKernel;

Console.WriteLine("Hello, World!");

const string API_KEY = "";
const string MODEL_ID = "Open-Orca/Mistral-7B-OpenOrca";

var kernel = new KernelBuilder().WithTogetherAIService(modelId: MODEL_ID, apiKey: API_KEY).Build();

const string promptTemplate =
    "<|im_start|> system\nDoes the text contain grammar errors? Answer with (Y/N)\n\n'{{$input}}'\n<|im_end|>\n<|im_start|> assistant\n";

var grammarValidation = kernel.CreateSemanticFunction(promptTemplate);

var result = await grammarValidation.InvokeAsync(
    input: "I missed the training session this morning",
    kernel: kernel
);

Console.WriteLine(result.GetValue<string>());
