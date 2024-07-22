using System.Text.Json.Serialization;

namespace Anthropic.ApiModels.SharedModels;

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