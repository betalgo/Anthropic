using System.Text.Json.Serialization;
using Betalgo.Anthropic.ApiModels.ResponseModels;

namespace Betalgo.Anthropic.ApiModels.SharedModels;

public class ImageSource : TypeBaseResponse
{
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("data")]
    public string? Data { get; set; }
}