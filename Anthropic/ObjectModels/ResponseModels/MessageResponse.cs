using System.Text.Json.Serialization;
using Anthropic.ObjectModels.SharedModels;

namespace Anthropic.ObjectModels.ResponseModels;

public class PingResponse: BaseResponse, IStreamResponse
{
}
public class MessageResponse : BaseResponse, IStreamResponse
{

    public override string? ToString()
    {
        return Content?.FirstOrDefault()?.Text;
    }

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

    public Message ToMessage()
    {
        return new()
        {
            Role = Role,
            Content = Content
        };
    }
}