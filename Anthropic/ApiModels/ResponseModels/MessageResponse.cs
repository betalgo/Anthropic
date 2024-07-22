using System.Text.Json.Serialization;
using Anthropic.ApiModels.SharedModels;

namespace Anthropic.ApiModels.ResponseModels;

public class PingResponse : BaseResponse, IStreamResponse
{
}

public class MessageResponse : BaseResponse, IStreamResponse
{
    [JsonPropertyName("content")]
    [JsonConverter(typeof(JsonConverters.ContentConverter))]
    public List<ContentBlock>? Content { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }

    [JsonPropertyName("stop_reason")]
    public string? StopReason { get; set; }

    [JsonPropertyName("stop_sequence")]
    public string? StopSequence { get; set; }

    public override string? ToString()
    {
        return Content?.FirstOrDefault()?.Text;
    }

    public Message ToMessage()
    {
        return new()
        {
            Role = Role,
            Content = Content
        };
    }
}