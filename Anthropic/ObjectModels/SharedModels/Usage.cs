using System.Text.Json.Serialization;

namespace Anthropic.ObjectModels.SharedModels;

public class Usage
{
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }
}