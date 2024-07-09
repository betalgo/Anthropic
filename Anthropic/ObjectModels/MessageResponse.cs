﻿using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Anthropic.ObjectModels;

/// <summary>
///     Represents a response from the Anthropic API for a message request.
/// </summary>
public class MessageResponse:BaseResponse
{
    /// <summary>
    ///     Gets or sets the conversational role of the generated message. This will always be "assistant".
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; set; }

    /// <summary>
    ///     Gets or sets the content generated by the model.
    /// </summary>
    [JsonPropertyName("content")]
    public List<ContentBlock> Content { get; set; }

    /// <summary>
    ///     Gets or sets the model that handled the request.
    /// </summary>
    [JsonPropertyName("model")]
    public string Model { get; set; }

    /// <summary>
    ///     Gets or sets the reason that the generation stopped.
    /// </summary>
    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; }

    /// <summary>
    ///     Gets or sets the custom stop sequence that was generated, if any.
    /// </summary>
    [JsonPropertyName("stop_sequence")]
    public string StopSequence { get; set; }
}

/// <summary>
///     Represents a content block in the response.
/// </summary>
public class ContentBlock: TypeBaseResponse
{
    /// <summary>
    ///     Gets or sets the text content of the block.
    /// </summary>
    [JsonPropertyName("text")]
    public string Text { get; set; }
    [JsonPropertyName("name")]
    public string Name{ get; set; }

    public string Id { get; set; }
}

/// <summary>
///     Represents the usage information for billing and rate-limiting.
/// </summary>
public class Usage
{
    /// <summary>
    ///     Gets or sets the number of input tokens used.
    /// </summary>
    [JsonPropertyName("input_tokens")]
    public int InputTokens { get; set; }

    /// <summary>
    ///     Gets or sets the number of output tokens used.
    /// </summary>
    [JsonPropertyName("output_tokens")]
    public int OutputTokens { get; set; }
}