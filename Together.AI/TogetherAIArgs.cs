#if NET462
using Newtonsoft.Json;

namespace Together.AI
{
    public class TogetherAIArgs
    {
        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("prompt")]
        public string Prompt { get; set; }

        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("top_p")]
        public double TopP { get; set; }

        [JsonProperty("top_k")]
        public long TopK { get; set; }

        [JsonProperty("max_tokens")]
        public long MaxTokens { get; set; }

        [JsonProperty("repetition_penalty")]
        public long RepetitionPenalty { get; set; }
    }

    public class TogetherAIRequestArgs : TogetherAIArgs
    {
        [JsonProperty("stream_tokens")]
        public bool StreamTokens { get; set; } = false;
    }
}
#else
using System.Text.Json.Serialization;

namespace Together.AI
{
    public class TogetherAIArgs
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("prompt")]
        public string Prompt { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }

        [JsonPropertyName("top_p")]
        public double TopP { get; set; }

        [JsonPropertyName("top_k")]
        public long TopK { get; set; }

        [JsonPropertyName("max_tokens")]
        public long MaxTokens { get; set; }

        [JsonPropertyName("repetition_penalty")]
        public long RepetitionPenalty { get; set; }
    }

    public class TogetherAIRequestArgs : TogetherAIArgs
    {
        [JsonPropertyName("stream_tokens")]
        public bool StreamTokens { get; set; } = false;
    }
}
#endif
