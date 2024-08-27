using System.Text.Json.Serialization;

namespace Betalgo.Anthropic.ApiModels.SharedModels;

/// <summary>
///     Represents metadata about the request.
/// </summary>
public class Metadata
{
    /// <summary>
    ///     Gets or sets an external identifier for the user associated with the request.
    /// </summary>
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
}