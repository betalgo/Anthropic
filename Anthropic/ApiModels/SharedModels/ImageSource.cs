using System.Text.Json.Serialization;
using Anthropic.ObjectModels.ResponseModels;

namespace Anthropic.ObjectModels.SharedModels;

public class ImageSource : TypeBaseResponse
{
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}