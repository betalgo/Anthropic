using System.Net;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Anthropic.Extensions;

namespace Anthropic.ObjectModels;
public class TypeBaseResponse
{
    /// <summary>
    ///     Gets or sets the object type. For Messages, this is always "message".
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

}
public class BaseResponse: TypeBaseResponse
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
    public Usage Usage { get; set; }

    [JsonPropertyName("StreamEvent")]
    public string? StreamEvent { get; set; }
    public bool IsDelta => StreamEvent?.EndsWith("delta") ?? false;

    public HttpStatusCode HttpStatusCode { get; set; }
    public ResponseHeaderValues? HeaderValues { get; set; }

    [JsonPropertyName("error")]
    public Error? Error { get; set; }

    public bool Successful => Error == null;
}

public class Error
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// Represents the values of various headers in an HTTP response.
/// </summary>
public class ResponseHeaderValues
{
    /// <summary>
    /// Gets the date and time at which the message was originated.
    /// </summary>
    public DateTimeOffset? Date { get; set; }

    /// <summary>
    /// Gets the media type of the body of the response.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    /// Gets the transfer encoding applied to the body of the response.
    /// </summary>
    public string? TransferEncoding { get; set; }

    /// <summary>
    /// Gets the connection type of the response.
    /// </summary>
    public string? Connection { get; set; }

    /// <summary>
    /// Gets the unique identifier for the request.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Gets the cloud trace context for the request.
    /// </summary>
    public string? XCloudTraceContext { get; set; }

    /// <summary>
    /// Gets information about proxies used in handling the request.
    /// </summary>
    public string? Via { get; set; }

    /// <summary>
    /// Gets the Cloudflare cache status for the request.
    /// </summary>
    public string? CFCacheStatus { get; set; }

    /// <summary>
    /// Gets information about the server that handled the request.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    /// Gets the Cloudflare Ray ID for the request.
    /// </summary>
    public string? CF_RAY { get; set; }

    /// <summary>
    /// Gets the content encoding applied to the body of the response.
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    /// Gets a dictionary containing all headers from the response.
    /// </summary>
    public Dictionary<string, IEnumerable<string>>? All { get; set; }

    /// <summary>
    /// Gets the Anthropic-specific headers from the response.
    /// </summary>
    public AnthropicHeaders? Anthropic { get; set; }

    /// <summary>
    /// Initializes a new instance of the ResponseHeaderValues class from an HttpResponseMessage.
    /// </summary>
    /// <param name="response">The HTTP response message containing the headers.</param>
    public ResponseHeaderValues(HttpResponseMessage response)
    {
        Date = response.Headers.Date;
        ContentType = response.Content.Headers.ContentType?.ToString();
        TransferEncoding = response.Headers.TransferEncoding?.ToString();
        Connection = response.Headers.Connection?.ToString();
        RequestId = response.Headers.GetHeaderValue("request-id");
        XCloudTraceContext = response.Headers.GetHeaderValue("x-cloud-trace-context");
        Via = response.Headers.Via?.ToString();
        CFCacheStatus = response.Headers.GetHeaderValue("cf-cache-status");
        Server = response.Headers.Server?.ToString();
        CF_RAY = response.Headers.GetHeaderValue("cf-ray");
        ContentEncoding = response.Content.Headers.ContentEncoding?.ToString();
        All = response.Headers.ToDictionary(x => x.Key, x => x.Value.AsEnumerable());
        Anthropic = new AnthropicHeaders(response.Headers);
    }
}

/// <summary>
/// Represents Anthropic-specific headers in an HTTP response.
/// </summary>
public class AnthropicHeaders
{
    /// <summary>
    /// Gets information about rate limits applied to the request.
    /// </summary>
    public RateLimitInfo? RateLimit { get; }

    /// <summary>
    /// Initializes a new instance of the AnthropicHeaders class from HttpResponseHeaders.
    /// </summary>
    /// <param name="headers">The HTTP response headers.</param>
    public AnthropicHeaders(HttpResponseHeaders headers)
    {
        RateLimit = new RateLimitInfo(headers);
    }
}

/// <summary>
/// Represents rate limit information from Anthropic API headers.
/// </summary>
public class RateLimitInfo
{
    /// <summary>
    /// Gets the maximum number of requests allowed within any rate limit period.
    /// </summary>
    public int? RequestsLimit { get; }

    /// <summary>
    /// Gets the number of requests remaining before being rate limited.
    /// </summary>
    public int? RequestsRemaining { get; }

    /// <summary>
    /// Gets the time when the request rate limit will reset, provided in RFC 3339 format.
    /// </summary>
    public DateTimeOffset? RequestsReset { get; }

    /// <summary>
    /// Gets the maximum number of tokens allowed within any rate limit period.
    /// </summary>
    public int? TokensLimit { get; }

    /// <summary>
    /// Gets the number of tokens remaining (rounded to the nearest thousand) before being rate limited.
    /// </summary>
    public int? TokensRemaining { get; }

    /// <summary>
    /// Gets the time when the token rate limit will reset, provided in RFC 3339 format.
    /// </summary>
    public DateTimeOffset? TokensReset { get; }

    /// <summary>
    /// Gets the number of seconds until you can retry the request.
    /// </summary>
    public int? RetryAfter { get; }

    /// <summary>
    /// Initializes a new instance of the RateLimitInfo class from HttpResponseHeaders.
    /// </summary>
    /// <param name="headers">The HTTP response headers.</param>
    public RateLimitInfo(HttpResponseHeaders headers)
    {
        RequestsLimit = ParseIntHeader(headers, "anthropic-ratelimit-requests-limit");
        RequestsRemaining = ParseIntHeader(headers, "anthropic-ratelimit-requests-remaining");
        RequestsReset = ParseDateTimeOffsetHeader(headers, "anthropic-ratelimit-requests-reset");
        TokensLimit = ParseIntHeader(headers, "anthropic-ratelimit-tokens-limit");
        TokensRemaining = ParseIntHeader(headers, "anthropic-ratelimit-tokens-remaining");
        TokensReset = ParseDateTimeOffsetHeader(headers, "anthropic-ratelimit-tokens-reset");
        RetryAfter = ParseIntHeader(headers, "retry-after");
    }

    private static int? ParseIntHeader(HttpResponseHeaders headers, string headerName)
    {
        if (int.TryParse(headers.GetHeaderValue(headerName), out int value))
        {
            return value;
        }
        return null;
    }

    private static DateTimeOffset? ParseDateTimeOffsetHeader(HttpResponseHeaders headers, string headerName)
    {
        if (DateTimeOffset.TryParse(headers.GetHeaderValue(headerName), out DateTimeOffset value))
        {
            return value;
        }
        return null;
    }
}