using System.Text.Json.Serialization;

namespace Together.AI;

public record TogetherAIResult
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("prompt")]
    public string[]? Prompt { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("model_owner")]
    public string? ModelOwner { get; set; }

    [JsonPropertyName("tags")]
    public object? Tags { get; set; }

    [JsonPropertyName("num_returns")]
    public long? NumReturns { get; set; }

    [JsonPropertyName("args")]
    public TogetherAIArgs? Args { get; set; }

    [JsonPropertyName("subjobs")]
    public object[]? Subjobs { get; set; }

    [JsonPropertyName("output")]
    public TogetherAIOutput? Output { get; set; }
}

public record TogetherAIStreamResult
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("choices")]
    public TogetherAIChoice[]? Choices { get; set; }

    [JsonPropertyName("result_type")]
    public string? ResultType { get; set; }
}

public record TogetherAIOutput
{
    [JsonPropertyName("choices")]
    public TogetherAIChoice[]? Choices { get; set; }

    [JsonPropertyName("raw_compute_time")]
    public double? RawComputeTime { get; set; }

    [JsonPropertyName("result_type")]
    public string? ResultType { get; set; }
}

public record TogetherAIChoice
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("index")]
    public long? Index { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}
