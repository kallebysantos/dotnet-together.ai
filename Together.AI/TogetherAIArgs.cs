using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Together.AI;

public record TogetherAIModelArgs
{
    /// <summary>
    /// The name of the model to query.
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

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

public record TogetherAIStreamArgs : TogetherAIModelArgs
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

[Obsolete("Represents the legacy 'inference' endpoint request, please use the newer implementation.")]
public record TogetherAIRequestArgs : TogetherAIStreamArgs
{
    /// <summary>
    /// A string providing context for the model to complete.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }
}

public record TogetherAICompletionArgs : TogetherAIStreamArgs
{
    /// <summary>
    /// A string providing context for the model to complete.
    /// </summary>
    [JsonPropertyName("prompt")]
    public string? Prompt { get; set; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = "role")]
[JsonDerivedType(typeof(TogetherAIChatSystemMessage), typeDiscriminator: "system")]
[JsonDerivedType(typeof(TogetherAIChatUserMessage), typeDiscriminator: "user")]
[JsonDerivedType(typeof(TogetherAIChatAssistantMessage), typeDiscriminator: "assistant")]
[JsonDerivedType(typeof(TogetherAIChatToolCallMessage), typeDiscriminator: "tool")]
public abstract record TogetherAIChatMessage
{
    /// <summary>
    /// The role of the messages author. Choice between: system, user, or assistant
    /// </summary>
    [JsonIgnore]
    public string? Role { get; protected set; }

    /// <summary>
    /// The contents of the message.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; protected set; }
}

public record TogetherAIChatUserMessage : TogetherAIChatMessage
{
    /// <summary>
    /// Represent the user role of the message
    /// </summary>
    public static string UserRole = "user";

    public TogetherAIChatUserMessage(string Content)
    {
        base.Role = UserRole;
        base.Content = Content;
    }
}

public record TogetherAIChatSystemMessage : TogetherAIChatMessage
{
    /// <summary>
    /// Represent the system role of the message
    /// </summary>
    public static string SystemRole = "system";

    public TogetherAIChatSystemMessage(string Content)
    {
        base.Role = SystemRole;
        base.Content = Content;
    }
}

public record TogetherAIChatAssistantMessage : TogetherAIChatMessage
{
    /// <summary>
    /// Represent the assistant role of the message
    /// </summary>
    public static string AssistantRole = "assistant";

    public TogetherAIChatAssistantMessage(string Content)
    {
        base.Role = AssistantRole;
        base.Content = Content;
    }
}

public record TogetherAIChatToolCallMessage : TogetherAIChatMessage
{
    /// <summary>
    /// Represent the tool role of the message
    /// </summary>
    public static string ToolRole = "tool";

    /// <summary>
    /// The generated call Id
    /// </summary>
    [JsonPropertyName("tool_call_id")]
    public string? Id { get; protected set; }

    /// <summary>
    /// The name of the called function. 
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; protected set; }

    public TogetherAIChatToolCallMessage(string Id, string FunctionName, object FunctionResponse)
    {
        this.Id = Id;
        this.Name = FunctionName;
        base.Role = ToolRole;
        base.Content = FunctionResponse.ToString();
    }

    public static TogetherAIChatToolCallMessage FromToolCall(TogetherAIToolCall toolCall, object functionResponse)
        => new
        (
            Id: toolCall.Id ?? throw new ArgumentNullException("The given tool call don't have a call Id"),
            FunctionName: toolCall.Function?.Name ?? throw new ArgumentNullException("The given tool call don't have a function name"),
            FunctionResponse: JsonSerializer.Serialize(
                value: functionResponse,
                options: new JsonSerializerOptions(JsonSerializerDefaults.Web)
                {
                    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.WriteAsString
                }
            )
        );
}

public record TogetherAITool
{
    /// <summary>
    /// The type of the tool. Currently, only function is supported.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; } = TogetherAIFunctionTool.TypeName;

    /// <summary>
    /// The function tool object descriptor.
    /// </summary>
    [JsonPropertyName("function")]
    public TogetherAIFunctionTool? Function { get; set; }
}

public record TogetherAIFunctionTool
{
    public static string TypeName = "function";

    /// <summary>
    /// The name of the function to be called. Must be a-z, A-Z, 0-9,
    /// or contain underscores and dashes, with a maximum length of 64.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// A description of what the function does, used by the model to choose 
    /// when and how to call the function.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// The parameters the functions accepts, described as a JSON Schema object.
    /// </summary>
    [JsonPropertyName("parameters")]
    public IDictionary<string, dynamic>? Parameters { get; set; }
}

public interface ITogetherAIToolChoice;

public record TogetherAIAutoToolChoice : ITogetherAIToolChoice
{
    public override string ToString() => "auto";
}

public record TogetherAIToolChoice : ITogetherAIToolChoice
{
    /// <summary>
    /// The type of the tool. Currently, only function is supported.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; } = TogetherAIFunctionToolChoice.TypeName;

    /// <summary>
    /// The function tool object descriptor.
    /// </summary>
    [JsonPropertyName("function")]
    public TogetherAIFunctionToolChoice? Function { get; set; }
}

public record TogetherAIFunctionToolChoice
{
    public static string TypeName = "function";

    /// <summary>
    /// The name of the function to be called. Must be a-z, A-Z, 0-9,
    /// or contain underscores and dashes, with a maximum length of 64.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}

public record TogetherAIResponseFormat
{
    /// <summary>
    /// Type of response format. Currently only supports json_object.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; set; } = "json_object";

    /// <summary>
    /// A valid JSON schema. Can be used to force a specific response schema.
    /// </summary>
    [JsonPropertyName("schema")]
    public IDictionary<string, object>? Schema { get; set; }
}

public record TogetherAIChatCompletionArgs : TogetherAIStreamArgs
{
    /// <summary>
    /// A list of messages comprising the conversation so far.
    /// </summary>
    [JsonPropertyName("messages")]
    public IEnumerable<TogetherAIChatMessage>? Messages { get; set; }

    /// <summary>
    /// A list of tools the model may call. Currently, only functions are supported as a tool.
    /// Use this to provide a list of functions the model may generate JSON inputs for.
    /// </summary>
    [JsonPropertyName("tools")]
    public IEnumerable<TogetherAITool>? Tools { get; set; }

    /// <summary>
    /// Controls which (if any) function is called by the model. auto means the model can pick
    /// between generating a message or calling a function. Specifying a particular function
    /// via {"type": "function", "function": {"name": "my_function"}} forces the model to call 
    /// that function. auto is the default.
    /// </summary>
    [JsonPropertyName("tool_choice")]
    public ITogetherAIToolChoice? ToolChoice { get; set; }

    /// <summary>
    /// An object specifying the format that the model must output.
    /// Accepted value: json_object.
    /// </summary>
    [JsonPropertyName("response_format")]
    public TogetherAIResponseFormat? ResponseFormat { get; set; }
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
