using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Anthropic.ObjectModels.SharedModels;

namespace Anthropic.ObjectModels.ResponseModels;

public class TypeBaseResponse : IType
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public interface IType
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
}

public interface IStreamResponse : IType
{
    public string? StreamEvent { get; set; }
}

public static class StreamResponseExtensions
{
    public static T As<T>(this IStreamResponse response) where T : class, IStreamResponse
    {
        if (response is T typedResponse)
        {
            return typedResponse;
        }

        throw new InvalidCastException($"Cannot cast {response.GetType().Name} to {typeof(T).Name}");
    }

    public static bool TryAs<T>(this IStreamResponse response, out T result) where T : class, IStreamResponse
    {
        if (response is T typedResponse)
        {
            result = typedResponse;
            return true;
        }

        result = null;
        return false;
    }
}

public static class StreamExtensions
{
    public static bool IsMessageResponse(this IStreamResponse response)
    {
        return response.Type == "message";
    }

    public static bool IsPingResponse(this IStreamResponse response)
    {
        return response.Type == "ping";
    }

    public static bool IsError(this IStreamResponse response)
    {
        return response.Type == "error";
    }
}

public class BaseResponse : TypeBaseResponse, IStreamResponse
{
    /// <summary>
    ///     Gets or sets the unique object identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }

    /// <summary>
    ///     Gets or sets the usage information for billing and rate-limiting.
    /// </summary>
    [JsonPropertyName("usage")]
    public Usage? Usage { get; set; }

    public bool IsDelta => StreamEvent?.EndsWith("delta") ?? false;

    public HttpStatusCode HttpStatusCode { get; set; }
    public ResponseHeaderValues? HeaderValues { get; set; }

    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    public bool Successful => Error == null;

    [JsonPropertyName("StreamEvent")]
    public string? StreamEvent { get; set; }
}

public class Error : TypeBaseResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
///     Represents Anthropic-specific headers in an HTTP response.
/// </summary>
/// <remarks>
///     Initializes a new instance of the AnthropicHeaders class from HttpResponseHeaders.
/// </remarks>
public class AnthropicHeaders
{
    /// <summary>
    ///     Represents Anthropic-specific headers in an HTTP response.
    /// </summary>
    /// <remarks>
    ///     Initializes a new instance of the AnthropicHeaders class from HttpResponseHeaders.
    /// </remarks>
    /// <param name="headers">The HTTP response headers.</param>
    public AnthropicHeaders(HttpResponseHeaders headers)
    {
        RateLimit = new(headers);
    }

    /// <summary>
    ///     Gets information about rate limits applied to the request.
    /// </summary>
    public RateLimitInfo? RateLimit { get; }
}