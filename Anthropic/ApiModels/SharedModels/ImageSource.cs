using System.Text.Json.Serialization;
using Anthropic.ApiModels.ResponseModels;

namespace Anthropic.ApiModels.SharedModels;

public class ImageSource : TypeBaseResponse
{
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}