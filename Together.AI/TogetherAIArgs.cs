using System.Text.Json.Serialization;

namespace Together.AI;

public record TogetherAIArgs
{
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }

    [JsonPropertyName("temperature")]
    public double? Temperature { get; set; }

    [JsonPropertyName("top_p")]
    public double? TopP { get; set; }

    [JsonPropertyName("top_k")]
    public long? TopK { get; set; }

    [JsonPropertyName("max_tokens")]
    public long? MaxTokens { get; set; }

    [JsonPropertyName("repetition_penalty")]
    public long? RepetitionPenalty { get; set; }
}

public record TogetherAIRequestArgs : TogetherAIArgs
{
    [JsonPropertyName("stream_tokens")]
    public bool StreamTokens { get; set; } = false;
}
