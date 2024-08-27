using System.Text.Json.Serialization;

namespace Betalgo.Anthropic.ApiModels.SharedModels;

/// <summary>
///     Represents a tool definition that the model may use.
/// </summary>
public class Tool
{
    /// <summary>
    ///     Gets or sets the name of the tool.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>
    ///     Gets or sets the description of the tool.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the JSON schema for the tool's input.
    /// </summary>
    [JsonPropertyName("input_schema")]
    public PropertyDefinition InputSchema { get; set; }
}

/// <summary>
///     Represents the JSON schema for a tool's input.
/// </summary>
public class InputSchema
{
    /// <summary>
    ///     Gets or sets the type of the input schema.
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>
    ///     Gets or sets the properties of the input schema.
    /// </summary>
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; }
}