using System;
using System.Text.Json.Serialization;

namespace Together.AI;

[Obsolete("Represents the legacy 'inference' endpoint result, please use the newer implementation.")]
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

[Obsolete("Represents the legacy 'inference' endpoint result, please use the newer implementation.")]
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

public record TogetherAIChoiceItem
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }
}

[Obsolete("Represents the legacy 'choice' result, please use the newer implementation.")]
public record TogetherAIChoice : TogetherAIChoiceItem
{
    [JsonPropertyName("index")]
    public long? Index { get; set; }

    [JsonPropertyName("finish_reason")]
    public string? FinishReason { get; set; }
}


public record TogetherAIEmbeddingsResult
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("data")]
    public TogetherAIEmbeddings[]? Data { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }
}

public record TogetherAIEmbeddings
{
    [JsonPropertyName("object")]
    public string? Object { get; set; }

    [JsonPropertyName("embedding")]
    public float[]? Values { get; set; }

    [JsonPropertyName("index")]
    public long? Index { get; set; }
}
