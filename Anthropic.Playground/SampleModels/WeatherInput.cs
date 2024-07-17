using System.Text.Json.Serialization;

namespace Anthropic.Playground.SampleModels;

public class WeatherInput
{
    [JsonPropertyName("location")]
    public string Location { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }
}