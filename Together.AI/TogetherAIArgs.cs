using System;
using System.Text.Json.Serialization;

namespace Together.AI;

public record TogetherAIArgs
{
    /// <summary>
    /// The name of the model to query.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// A string providing context for the model to complete.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    /// <summary>
    /// The maximum number of tokens to generate.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public long? MaxTokens { get; set; }

    /// <summary>
    /// A list of string sequences that will truncate (stop) inference text output.
    /// For example, "" will stop generation as soon as the model generates the given token.
    /// </summary>
    [JsonPropertyName("stop")]
    public string[]? Stop { get; set; }

    /// <summary>
    /// A decimal number that determines the degree of randomness in the response. 
    /// A value of 1 will always yield the same output. 
    /// A temperature less than 1 favors more correctness and is appropriate for question 
    /// answering or summarization. 
    /// A value greater than 1 introduces more randomness in the output.
    /// </summary>
    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    /// <summary>
    /// The top_p (nucleus) parameter is used to dynamically adjust the number of choices for each predicted
    /// token based on the cumulative probabilities. It specifies a probability threshold, 
    /// below which all less likely tokens are filtered out. 
    /// This technique helps to maintain diversity and generate more fluent and natural-sounding text.
    /// </summary>
    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    /// <summary>
    /// The top_k parameter is used to limit the number of choices for the next predicted word or token.
    /// It specifies the maximum number of tokens to consider at each step, based on their probability of occurrence. 
    /// This technique helps to speed up the generation process and can improve the quality of the 
    /// generated text by focusing on the most likely options.
    /// </summary>
    [JsonPropertyName("top_k")]
    public long? TopK { get; set; }

    /// <summary>
    /// A number that controls the diversity of generated text by reducing the likelihood of repeated sequences.
    /// Higher values decrease repetition.
    /// </summary>
    [JsonPropertyName("repetition_penalty")]
    public long? RepetitionPenalty { get; set; }

    /// <summary>
    /// Number of top-k logprobs to return
    /// </summary>
    [JsonPropertyName("logprobs")]
    public long? Logprobs { get; set; }

    /// <summary>
    /// Echo prompt in output. Can be used with logprobs to return prompt logprobs.
    /// </summary>
    [JsonPropertyName("echo")]
    public bool? Echo { get; set; }

    /// <summary>
    /// How many completions to generate for each prompt
    /// </summary>
    [JsonPropertyName("n")]
    public int? NCompletions { get; set; }

    /// <summary>
    /// A moderation model to validate tokens. Choice between available moderation models found at:
    /// https://docs.together.ai/docs/inference-models#moderation-models
    /// </summary>
    [JsonPropertyName("safety_model")]
    public string? SafetyModel { get; set; }
}

public record TogetherAIRequestArgs : TogetherAIArgs
{
    /// <summary>
    /// If true, stream tokens as Server-Sent Events as the model generates them instead of 
    /// waiting for the full model response.
    /// If false, return a single JSON object containing the results.
    /// </summary>
    [Obsolete("Represents the legacy 'stream_tokens' argument, please use the newer implementation.")]
    [JsonPropertyName("stream_tokens")]
    public bool StreamTokens { get; set; } = false;

    /// <summary>
    /// If true, stream tokens as Server-Sent Events as the model generates them instead of 
    /// waiting for the full model response.
    /// If false, return a single JSON object containing the results.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; } = false;
}

public record TogetherAIEmbeddingsRequestArgs
{
    /// <summary>
    /// The name of the embedding model to use.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    /// <summary>
    /// A string providing the text for the model to embed.
    /// </summary>
    [JsonPropertyName("input")]
    public string? Input { get; set; }
}
