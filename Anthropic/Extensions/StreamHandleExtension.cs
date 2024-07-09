using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic.ObjectModels;

namespace Anthropic.Extensions;
public static class StreamHandleExtension
{
    public static async IAsyncEnumerable<MessageResponse> AsStream(this HttpResponseMessage response, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var reader = new StreamReader(stream);

        string? currentEvent = null;

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith("event: "))
            {
                currentEvent = line.Substring(7);
            //    yield return new() { StreamEvent = currentEvent };
            }
            else if (line.StartsWith("data: "))
            {
                var data = line.Substring(6);
                var jsonElement = JsonSerializer.Deserialize<JsonElement>(data);
                var eventType = jsonElement.GetProperty("type").GetString();

                switch (eventType)
                {
                    case "content_block_delta":
                        var contentBlockDelta = JsonSerializer.Deserialize<ContentBlockDelta>(data);
                        if (contentBlockDelta.Delta.Type == "text_delta")
                        {
                            yield return new()
                            {
                                StreamEvent = "content_block_delta",
                                Content = [new() { Text = contentBlockDelta.Delta.Text }]
                            };
                        }
                        break;
                    case "message_start":
                    case "message_delta":
                    case "message_stop":
                        yield return JsonSerializer.Deserialize<MessageResponse>(data) ?? new MessageResponse();
                        break;
                    default:
                        yield return JsonSerializer.Deserialize<MessageResponse>(data) ?? new MessageResponse();
                        break;
                }
            }
        }
    }
}

public class Delta<T>
{
    [JsonPropertyName("index")]
    public int Index { get; set; }
}
public class ContentBlockStart
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("content_block")]
    public ContentBlock ContentBlock { get; set; }
}

public class ContentBlockDelta
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("delta")]
    public Delta Delta { get; set; }
}

public class Delta
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("text")]
    public string Text { get; set; }
}

public class MessageDelta
{
    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("delta")]
    public DeltaInfo Delta { get; set; }

    [JsonPropertyName("usage")]
    public Usage Usage { get; set; }
}

public class DeltaInfo
{
    [JsonPropertyName("stop_reason")]
    public string StopReason { get; set; }

    [JsonPropertyName("stop_sequence")]
    public string StopSequence { get; set; }
}