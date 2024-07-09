using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Anthropic.ObjectModels;

/// <summary>
///     Represents a request to create a message using the Anthropic API.
/// </summary>
public class MessageRequest : IObjectModels.IModel
{
    /// <summary>
    ///     Gets or sets the maximum number of tokens to generate before stopping.
    /// </summary>
    [JsonPropertyName("max_tokens")]
    public int MaxTokens { get; set; }

    /// <summary>
    ///     Gets or sets the input messages for the conversation.
    /// </summary>
    [JsonPropertyName("messages")]
    public List<Message> Messages { get; set; }

    /// <summary>
    ///     Gets or sets the metadata about the request.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Metadata Metadata { get; set; }

    /// <summary>
    ///     Gets or sets the custom text sequences that will cause the model to stop generating.
    /// </summary>
    [JsonPropertyName("stop_sequences")]
    public List<string> StopSequences { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to incrementally stream the response using server-sent events.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool Stream { get; set; }

    /// <summary>
    ///     Gets or sets the system prompt.
    /// </summary>
    [JsonPropertyName("system")]
    public string System { get; set; }

    /// <summary>
    ///     Gets or sets the amount of randomness injected into the response.
    /// </summary>
    [JsonPropertyName("temperature")]
    public float Temperature { get; set; }

    /// <summary>
    ///     Gets or sets how the model should use the provided tools.
    /// </summary>
    [JsonPropertyName("tool_choice")]
    public ToolChoice ToolChoice { get; set; }

    /// <summary>
    ///     Gets or sets the definitions of tools that the model may use.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<Tool> Tools { get; set; }

    /// <summary>
    ///     Gets or sets the model that will complete the prompt.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }
}

public class Message
{
    private Message(string assistant, string content)
    {
        Role = assistant;
        Content = new(content);
    }

    private Message(string assistant, List<ContentBlock> contents)
    {
        Role = assistant;
        Content = new(contents);
    }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public MessageContentOneOfType Content { get; set; }


    public static Message FromAssistant(string content)
    {
        return new("assistant", content);
    }

    public static Message FromUser(string content)
    {
        return new("user", content);
    }

    public static Message FromUser(List<ContentBlock> contents)
    {
        return new("user", contents);
    }
}

/// <summary>
///     Represents metadata about the request.
/// </summary>
public class Metadata
{
    /// <summary>
    ///     Gets or sets an external identifier for the user associated with the request.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
}

/// <summary>
///     Represents how the model should use the provided tools.
/// </summary>
public class ToolChoice
{
    /// <summary>
    ///     Gets or sets the type of tool choice.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

/// <summary>
///     Represents a tool definition that the model may use.
/// </summary>
public class Tool
{
    /// <summary>
    ///     Gets or sets the name of the tool.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the tool.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the JSON schema for the tool's input.
    /// </summary>
    [JsonPropertyName("input_schema")]
    public InputSchema InputSchema { get; set; }
}

/// <summary>
///     Represents the JSON schema for a tool's input.
/// </summary>
public class InputSchema
{
    /// <summary>
    ///     Gets or sets the type of the input schema.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    ///     Gets or sets the properties of the input schema.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; }
}

public class MessageContentConverter : JsonConverter<MessageContentOneOfType>
{
    public override MessageContentOneOfType? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.TokenType switch
        {
            JsonTokenType.String => new() { AsString = reader.GetString() },
            JsonTokenType.StartArray => new() { AsBlocks = JsonSerializer.Deserialize<List<ContentBlock>>(ref reader, options) },
            _ => throw new JsonException("Invalid token type for MessageContent")
        };
    }

    public override void Write(Utf8JsonWriter writer, MessageContentOneOfType? value, JsonSerializerOptions options)
    {
        if (value?.AsString != null)
        {
            writer.WriteStringValue(value.AsString);
        }
        else if (value?.AsBlocks != null)
        {
            JsonSerializer.Serialize(writer, value.AsBlocks, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}

[JsonConverter(typeof(MessageContentConverter))]
public class MessageContentOneOfType
{
    public MessageContentOneOfType()
    {
    }

    public MessageContentOneOfType(string asString)
    {
        AsString = asString;
    }

    public MessageContentOneOfType(List<ContentBlock> asBlocks)
    {
        AsBlocks = asBlocks;
    }

    [JsonIgnore]
    public string? AsString { get; set; }

    [JsonIgnore]
    public List<ContentBlock>? AsBlocks { get; set; }
}