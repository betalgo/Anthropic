using System.Text.Json.Serialization;
using Anthropic.ObjectModels.ResponseModels;

namespace Anthropic.ObjectModels.SharedModels;

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
}