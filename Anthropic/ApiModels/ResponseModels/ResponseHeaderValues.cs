using Anthropic.Extensions;

namespace Anthropic.ObjectModels.ResponseModels;

/// <summary>
///     Represents the values of various headers in an HTTP response.
/// </summary>
/// <remarks>
///     Initializes a new instance of the ResponseHeaderValues class from an HttpResponseMessage.
/// </remarks>
public class ResponseHeaderValues
{
    /// <summary>
    ///     Represents the values of various headers in an HTTP response.
    /// </summary>
    /// <remarks>
    ///     Initializes a new instance of the ResponseHeaderValues class from an HttpResponseMessage.
    /// </remarks>
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
        Anthropic = new(response.Headers);
    }

    /// <summary>
    ///     Gets the date and time at which the message was originated.
    /// </summary>
    public DateTimeOffset? Date { get; set; }

    /// <summary>
    ///     Gets the media type of the body of the response.
    /// </summary>
    public string? ContentType { get; set; }

    /// <summary>
    ///     Gets the transfer encoding applied to the body of the response.
    /// </summary>
    public string? TransferEncoding { get; set; }

    /// <summary>
    ///     Gets the connection type of the response.
    /// </summary>
    public string? Connection { get; set; }

    /// <summary>
    ///     Gets the unique identifier for the request.
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    ///     Gets the cloud trace context for the request.
    /// </summary>
    public string? XCloudTraceContext { get; set; }

    /// <summary>
    ///     Gets information about proxies used in handling the request.
    /// </summary>
    public string? Via { get; set; }

    /// <summary>
    ///     Gets the Cloudflare cache status for the request.
    /// </summary>
    public string? CFCacheStatus { get; set; }

    /// <summary>
    ///     Gets information about the server that handled the request.
    /// </summary>
    public string? Server { get; set; }

    /// <summary>
    ///     Gets the Cloudflare Ray ID for the request.
    /// </summary>
    public string? CF_RAY { get; set; }

    /// <summary>
    ///     Gets the content encoding applied to the body of the response.
    /// </summary>
    public string? ContentEncoding { get; set; }

    /// <summary>
    ///     Gets a dictionary containing all headers from the response.
    /// </summary>
    public Dictionary<string, IEnumerable<string>>? All { get; set; }

    /// <summary>
    ///     Gets the Anthropic-specific headers from the response.
    /// </summary>
    public AnthropicHeaders? Anthropic { get; set; }
}