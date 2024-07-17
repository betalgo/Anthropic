using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic.ObjectModels.ResponseModels;

namespace Anthropic.Extensions;

internal static class HttpClientExtensions
{
    public static async Task<TResponse> GetReadAsAsync<TResponse>(this HttpClient client, string uri, CancellationToken cancellationToken = default) where TResponse : BaseResponse, new()
    {
        var response = await client.GetAsync(uri, cancellationToken);
        return await HandleResponseContent<TResponse>(response, cancellationToken);
    }

    public static async Task<TResponse> PostAndReadAsAsync<TResponse>(this HttpClient client, string uri, object? requestModel, CancellationToken cancellationToken = default) where TResponse : BaseResponse, new()
    {
        var response = await client.PostAsJsonAsync(uri, requestModel, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        }, cancellationToken);
        return await HandleResponseContent<TResponse>(response, cancellationToken);
    }

    public static string? GetHeaderValue(this HttpHeaders headers, string name)
    {
        return headers.TryGetValues(name, out var values) ? values.FirstOrDefault() : null;
    }

    public static HttpResponseMessage PostAsStreamAsync(this HttpClient client, string uri, object requestModel, CancellationToken cancellationToken = default)
    {
        var settings = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
        };

        var content = JsonContent.Create(requestModel, null, settings);

        using var request = CreatePostEventStreamRequest(uri, content);

#if NET6_0_OR_GREATER
        try
        {
            return client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        }
        catch (PlatformNotSupportedException)
        {
            using var newRequest = CreatePostEventStreamRequest(uri, content);

            return SendRequestPreNet6(client, newRequest, cancellationToken);
        }
#else
        return SendRequestPreNet6(client, request, cancellationToken);
#endif
    }

    private static HttpResponseMessage SendRequestPreNet6(HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var responseTask = client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        var response = responseTask.GetAwaiter().GetResult();
        return response;
    }

    private static HttpRequestMessage CreatePostEventStreamRequest(string uri, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Headers.Accept.Add(new("text/event-stream"));
        request.Content = content;

        return request;
    }

    public static async Task<TResponse> PostFileAndReadAsAsync<TResponse>(this HttpClient client, string uri, HttpContent content, CancellationToken cancellationToken = default) where TResponse : BaseResponse, new()
    {
        var response = await client.PostAsync(uri, content, cancellationToken);
        return await HandleResponseContent<TResponse>(response, cancellationToken);
    }

    public static async Task<(string? stringResponse, TResponse baseResponse)> PostFileAndReadAsStringAsync<TResponse>(this HttpClient client, string uri, HttpContent content, CancellationToken cancellationToken = default)
        where TResponse : BaseResponse, new()
    {
        var response = await client.PostAsync(uri, content, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var tResponse = new TResponse
            {
                HttpStatusCode = response.StatusCode,
                HeaderValues = new(response)
            };
            return (await response.Content.ReadAsStringAsync(cancellationToken), tResponse);
        }
        else
        {
            return (null, await HandleResponseContent<TResponse>(response, cancellationToken));
        }
    }

    public static async Task<TResponse> DeleteAndReadAsAsync<TResponse>(this HttpClient client, string uri, CancellationToken cancellationToken = default) where TResponse : BaseResponse, new()
    {
        var response = await client.DeleteAsync(uri, cancellationToken);
        return await HandleResponseContent<TResponse>(response, cancellationToken);
    }

    public static async Task<TResponse> HandleResponseContent<TResponse>(this HttpResponseMessage response, CancellationToken cancellationToken) where TResponse : BaseResponse, new()
    {
        TResponse result;

        if (!response.Content.Headers.ContentType?.MediaType?.Equals("application/json", StringComparison.OrdinalIgnoreCase) ?? true)
        {
            result = new()
            {
                Error = new()
                {
                    Type = "InvalidError",
                    Message = await response.Content.ReadAsStringAsync(cancellationToken)
                }
            };
        }
        else
        {
            result = await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken:cancellationToken) ?? throw new InvalidOperationException();
        }

        result.HttpStatusCode = response.StatusCode;
        result.HeaderValues = new(response);

        return result;
    }


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


#if NETSTANDARD2_0
    public static async Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken)
    {
        var stream = await content.ReadAsStreamAsync().WithCancellation(cancellationToken);
        using var sr = new StreamReader(stream);
        return await sr.ReadToEndAsync().WithCancellation(cancellationToken);
    }

    public static async Task<AsyncDisposableStream> ReadAsStreamAsync(this HttpContent content, CancellationToken cancellationToken)
    {
        var stream = await content.ReadAsStreamAsync().WithCancellation(cancellationToken);
        return new(stream);
    }

    public static async Task<byte[]> ReadAsByteArrayAsync(this HttpContent content, CancellationToken cancellationToken)
    {
        return await content.ReadAsByteArrayAsync().WithCancellation(cancellationToken);
    }

    public static async Task<Stream> GetStreamAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
    {
        var response = await client.GetAsync(requestUri, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }

    public static async Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<bool>();
        using (cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).TrySetResult(true), tcs))
        {
            if (task != await Task.WhenAny(task, tcs.Task))
            {
                throw new OperationCanceledException(cancellationToken);
            }
        }

        return await task;
    }
#endif
}