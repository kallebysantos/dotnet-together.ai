#if NET462
using Newtonsoft.Json;

namespace Together.AI
{
    public class TogetherAIResult
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("prompt")]
        public string[] Prompt { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("model_owner")]
        public string ModelOwner { get; set; }

        [JsonProperty("tags")]
        public object Tags { get; set; }

        [JsonProperty("num_returns")]
        public long NumReturns { get; set; }

        [JsonProperty("args")]
        public TogetherAIArgs Args { get; set; }

        [JsonProperty("subjobs")]
        public object[] Subjobs { get; set; }

        [JsonProperty("output")]
        public TogetherAIOutput Output { get; set; }
    }

    public class TogetherAIStreamResult
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("choices")]
        public TogetherAIChoice[] Choices { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }
    }

    public class TogetherAIOutput
    {
        [JsonProperty("choices")]
        public TogetherAIChoice[] Choices { get; set; }

        [JsonProperty("raw_compute_time")]
        public double RawComputeTime { get; set; }

        [JsonProperty("result_type")]
        public string ResultType { get; set; }
    }

    public class TogetherAIChoice
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("index")]
        public long Index { get; set; }

        [JsonProperty("finish_reason")]
        public string FinishReason { get; set; }
    }
}
#else
using System.Text.Json.Serialization;

namespace Together.AI
{
    public class TogetherAIResult
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("prompt")]
        public string[] Prompt { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("model_owner")]
        public string ModelOwner { get; set; }

        [JsonPropertyName("tags")]
        public object Tags { get; set; }

        [JsonPropertyName("num_returns")]
        public long NumReturns { get; set; }

        [JsonPropertyName("args")]
        public TogetherAIArgs Args { get; set; }

        [JsonPropertyName("subjobs")]
        public object[] Subjobs { get; set; }

        [JsonPropertyName("output")]
        public TogetherAIOutput Output { get; set; }
    }

    public class TogetherAIStreamResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("choices")]
        public TogetherAIChoice[] Choices { get; set; }

        [JsonPropertyName("result_type")]
        public string ResultType { get; set; }
    }

    public class TogetherAIOutput
    {
        [JsonPropertyName("choices")]
        public TogetherAIChoice[] Choices { get; set; }

        [JsonPropertyName("raw_compute_time")]
        public double RawComputeTime { get; set; }

        [JsonPropertyName("result_type")]
        public string ResultType { get; set; }
    }

    public class TogetherAIChoice
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("index")]
        public long Index { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }
    }
}
#endif
