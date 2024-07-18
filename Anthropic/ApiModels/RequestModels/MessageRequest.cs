using System.Text.Json.Serialization;
using Anthropic.ObjectModels;
using Anthropic.ObjectModels.SharedModels;

namespace Anthropic.ApiModels.RequestModels;

/// <summary>
///     Represents a request to create a message using the Anthropic API.
/// </summary>
public class MessageRequest : IObjectInterfaces.IModel
{
    public MessageRequest()
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="messages"></param>
    /// <param name="maxTokens"></param>
    /// <param name="model">Model is required for the request but can be set later by using the default model.</param>
    public MessageRequest(List<Message> messages, int maxTokens, string? model = null)
    {
        Model = model;
        Messages = messages;
        MaxTokens = maxTokens;
    }


    /// <summary>
    ///     Gets or sets the maximum number of tokens to generate before stopping.
    /// </summary>
    /// <remarks> Required </remarks>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    /// <summary>
    ///     Gets or sets the input messages for the conversation.
    /// </summary>
    /// <remarks> Required </remarks>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }

    /// <summary>
    ///     Gets or sets the metadata about the request.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Metadata? Metadata { get; set; }

    /// <summary>
    ///     Gets or sets the custom text sequences that will cause the model to stop generating.
    /// </summary>
    [JsonPropertyName("stop_sequences")]
    public List<string>? StopSequences { get; set; }

    /// <summary>
    ///     The library will automatically set this value.
    ///     Gets or sets a value indicating whether to incrementally stream the response using server-sent events.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool? Stream { get; internal set; }

    /// <summary>
    ///     Gets or sets the system prompt.
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; set; }

    /// <summary>
    ///     Gets or sets the amount of randomness injected into the response.
    /// </summary>
    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    /// <summary>
    ///     Gets or sets how the model should use the provided tools.
    /// </summary>
    [JsonPropertyName("tool_choice")]
    public ToolChoice? ToolChoice { get; set; }

    /// <summary>
    ///     Gets or sets the definitions of tools that the model may use.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<Tool>? Tools { get; set; }

    /// <summary>
    ///     Gets or sets the model that will complete the prompt.
    /// </summary>
    /// <remarks> Required </remarks>
    [JsonPropertyName("model")]
    public string Model { get; set; }
}