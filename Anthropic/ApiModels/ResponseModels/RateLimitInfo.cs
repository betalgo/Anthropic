using System.Net.Http.Headers;
using Anthropic.Extensions;

namespace Anthropic.ApiModels.ResponseModels;

/// <summary>
///     Represents rate limit information from Anthropic API headers.
/// </summary>
/// <remarks>
///     Initializes a new instance of the RateLimitInfo class from HttpResponseHeaders.
/// </remarks>
public class RateLimitInfo
{
    /// <summary>
    ///     Represents rate limit information from Anthropic API headers.
    /// </summary>
    /// <remarks>
    ///     Initializes a new instance of the RateLimitInfo class from HttpResponseHeaders.
    /// </remarks>
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

    /// <summary>
    ///     Gets the maximum number of requests allowed within any rate limit period.
    /// </summary>
    public int? RequestsLimit { get; }

    /// <summary>
    ///     Gets the number of requests remaining before being rate limited.
    /// </summary>
    public int? RequestsRemaining { get; }

    /// <summary>
    ///     Gets the time when the request rate limit will reset, provided in RFC 3339 format.
    /// </summary>
    public DateTimeOffset? RequestsReset { get; }

    /// <summary>
    ///     Gets the maximum number of tokens allowed within any rate limit period.
    /// </summary>
    public int? TokensLimit { get; }

    /// <summary>
    ///     Gets the number of tokens remaining (rounded to the nearest thousand) before being rate limited.
    /// </summary>
    public int? TokensRemaining { get; }

    /// <summary>
    ///     Gets the time when the token rate limit will reset, provided in RFC 3339 format.
    /// </summary>
    public DateTimeOffset? TokensReset { get; }

    /// <summary>
    ///     Gets the number of seconds until you can retry the request.
    /// </summary>
    public int? RetryAfter { get; }

    private static int? ParseIntHeader(HttpResponseHeaders headers, string headerName)
    {
        if (int.TryParse(headers.GetHeaderValue(headerName), out var value))
        {
            return value;
        }

        return null;
    }

    private static DateTimeOffset? ParseDateTimeOffsetHeader(HttpResponseHeaders headers, string headerName)
    {
        if (DateTimeOffset.TryParse(headers.GetHeaderValue(headerName), out var value))
        {
            return value;
        }

        return null;
    }
}