using System.Text.Json.Serialization;
using Anthropic.ApiModels.ResponseModels;

namespace Anthropic.ApiModels.SharedModels;

public class ContentBlock : TypeBaseResponse
{
    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("source")]
    public ImageSource? Source { get; set; }

    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("input")]
    [JsonConverter(typeof(JsonConverters.InputJsonConverter))]
    public object? Input { get; set; }

    [JsonIgnore]
    public bool IsText => Type == "text";

    [JsonIgnore]
    public bool IsImage => Type == "image";

    [JsonIgnore]
    public bool IsToolUse => Type == "tool_use";

    [JsonPropertyName("tool_use_id")]

    public string? ToolUseId { get; set; }

    [JsonPropertyName("content")]

    public List<ContentBlock>? Content { get; set; }

    public static ContentBlock CreateText(string? text)
    {
        return new()
        {
            Type = "text",
            Text = text
        };
    }

    public static ContentBlock CreateImage(string mediaType, string data)
    {
        return new()
        {
            Type = "image",
            Source = new()
            {
                MediaType = mediaType,
                Data = data
            }
        };
    }

    public static ContentBlock CreateToolUse(object input, string? id, string? name)
    {
        return new()
        {
            Type = "tool_use",
            Id = id,
            Name = name,
            Input = input
        };
    }

    public static ContentBlock CreateToolResult(object input, string toolUseId, List<ContentBlock>? content)
    {
        return new()
        {
            Type = "tool_result",
            ToolUseId = toolUseId,
            Input = input,
            Content = content
        };
    }
}