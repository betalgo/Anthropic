using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic.ApiModels;
using Anthropic.ApiModels.ResponseModels;
using Anthropic.ApiModels.SharedModels;
using static Anthropic.Extensions.StreamPartialResponse;

namespace Anthropic.Extensions;

public static class StreamHandleExtension
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async IAsyncEnumerable<IStreamResponse> AsStream(this HttpResponseMessage response, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var reader = new StreamReader(stream);

        var currentEvent = string.Empty;
        var currentEventType = string.Empty;
        string? currentEventSubType = null;
        var toolUseBuilder = new StreamToolUseJsonBuilder();

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync();
            if (string.IsNullOrEmpty(line)) continue;

            if (line.StartsWith(StaticValues.StreamConstants.Event, StringComparison.Ordinal))
            {
#if NET6_0_OR_GREATER
                (currentEventType, currentEventSubType) = ParseEvent(line.AsSpan(StaticValues.StreamConstants.EventCharCount));
#else
                (currentEventType, currentEventSubType) = ParseEvent(line.Substring(StaticValues.StreamConstants.EventCharCount));
#endif
                currentEvent = line.Substring(StaticValues.StreamConstants.EventCharCount);
            }
            else if (line.StartsWith(StaticValues.StreamConstants.Data, StringComparison.Ordinal))
            {
                var data = line.Substring(StaticValues.StreamConstants.DataCharCount);
                var yieldResponse = ProcessEventData(data, currentEvent, currentEventType, currentEventSubType, toolUseBuilder);
                if (yieldResponse == null)
                {
                    continue;
                }

                yield return yieldResponse;
            }
        }
    }

    private static IStreamResponse? ProcessEventData(string data, string currentEvent, string currentEventType, string? currentEventSubType, StreamToolUseJsonBuilder toolUseBuilder)
    {
        return currentEventSubType switch
        {
            StaticValues.EventSubTypes.Ping => new PingResponse { StreamEvent = currentEvent },
            StaticValues.EventSubTypes.Start => ProcessStartEvent(data, currentEvent, currentEventType, toolUseBuilder),
            StaticValues.EventSubTypes.Delta => ProcessDeltaEvent(data, currentEvent, currentEventType, toolUseBuilder),
            StaticValues.EventSubTypes.Stop => ProcessStopEvent(data, currentEvent, currentEventType, toolUseBuilder),
            _ => DeserializeMessageResponse(data, currentEvent)
        };
    }

    private static IStreamResponse ProcessStartEvent(string data, string currentEvent, string currentEventType, StreamToolUseJsonBuilder toolUseBuilder)
    {
        if (currentEventType == StaticValues.TypeConstants.ContentBlock)
        {
            var startBlock = JsonSerializer.Deserialize<ContentBlockItem>(data, JsonOptions);
            if (startBlock.ContentBlock.Type == StaticValues.TypeConstants.ToolUse)
            {
                toolUseBuilder.StartBlock(startBlock.Index, startBlock.ContentBlock);
                return new MessageResponse();
            }

            return new MessageResponse
            {
                Type = StaticValues.TypeConstants.Message,
                StreamEvent = currentEvent,
                Content = [startBlock.ContentBlock]
            };
        }

        if (currentEventType == StaticValues.TypeConstants.Message)
        {
            var messageEvent = JsonSerializer.Deserialize<MessageEventItem>(data, JsonOptions);
            var response = messageEvent?.Message ?? new MessageResponse();
            response.StreamEvent = currentEvent;
            return response;
        }

        return DeserializeMessageResponse(data, currentEvent);
    }

    private static IStreamResponse? ProcessDeltaEvent(string data, string currentEvent, string currentEventType, StreamToolUseJsonBuilder toolUseBuilder)
    {
        var deltaBase = JsonSerializer.Deserialize<DeltaItem<TypeBaseResponse>>(data, JsonOptions);
        if (deltaBase?.Delta?.Type == null)
        {
            return null;
        }

        var (deltaEventType, deltaEventSubType) = ParseEvent(deltaBase.Delta.Type);

        switch (currentEventType)
        {
            case StaticValues.TypeConstants.Message:
            {
                var deltaItemMessage = JsonSerializer.Deserialize<DeltaItem<MessageResponse>>(data, JsonOptions);
                var response = deltaItemMessage!.Delta;
                if (response != null)
                {
                    response.Usage = deltaItemMessage.Usage;
                }

                return response;
            }
            case StaticValues.TypeConstants.ContentBlock when deltaEventSubType == StaticValues.EventSubTypes.Delta:
            {
                if (deltaEventType == StaticValues.TypeConstants.Text)
                {
                    var delta = JsonSerializer.Deserialize<DeltaItem<ContentBlock>>(data, JsonOptions);
                    return new MessageResponse
                    {
                        Type = StaticValues.TypeConstants.Message,
                        StreamEvent = currentEventType,
                        Content = [ContentBlock.CreateText(delta.Delta.Text)]
                    };
                }

                if (deltaEventType == StaticValues.TypeConstants.InputJson)
                {
                    var delta = JsonSerializer.Deserialize<DeltaItem<InputJsonDelta>>(data, JsonOptions);
                    toolUseBuilder.AppendJson(delta?.Index, delta?.Delta?.PartialJson);
                }

                break;
            }
        }

        return new MessageResponse();
    }

    private static IStreamResponse ProcessStopEvent(string data, string currentEvent, string currentEventType, StreamToolUseJsonBuilder toolUseBuilder)
    {
        if (currentEventType == StaticValues.TypeConstants.ContentBlock)
        {
            var stopBlock = JsonSerializer.Deserialize<ContentBlockItem>(data, JsonOptions);
            if (toolUseBuilder.IsActiveBlock(stopBlock.Index))
            {
                var finalBlock = toolUseBuilder.FinishBlock(stopBlock.Index);
                return new MessageResponse
                {
                    Type = StaticValues.TypeConstants.Message,
                    StreamEvent = currentEvent,
                    Content = [finalBlock]
                };
            }
        }

        return DeserializeMessageResponse(data, currentEvent);
    }

    private static MessageResponse DeserializeMessageResponse(string data, string currentEvent)
    {
        var response = JsonSerializer.Deserialize<MessageResponse>(data, JsonOptions) ?? new MessageResponse();
        response.StreamEvent = currentEvent;
        response.Type = StaticValues.TypeConstants.Message;
        return response;
    }

#if NET6_0_OR_GREATER
    private static (string EventType, string SubType) ParseEvent(ReadOnlySpan<char> eventName)
    {
        if (eventName.Equals("ping", StringComparison.Ordinal))
        {
            return ("ping", null);
        }

        var lastUnderscoreIndex = eventName.LastIndexOf('_');

        if (lastUnderscoreIndex != -1)
        {
            var eventType = eventName.Slice(0, lastUnderscoreIndex);
            var subType = eventName.Slice(lastUnderscoreIndex + 1);

            if (subType.Equals("delta", StringComparison.Ordinal) || subType.Equals("start", StringComparison.Ordinal) || subType.Equals("stop", StringComparison.Ordinal))
            {
                return (eventType.ToString(), subType.ToString());
            }
        }

        return (eventName.ToString(), null);
    }
#else
    private static (string EventType, string? SubType) ParseEvent(string eventName)
    {
        if (string.Equals(eventName, StaticValues.EventSubTypes.Ping, StringComparison.Ordinal))
        {
            return (StaticValues.EventSubTypes.Ping, null);
        }

        var lastUnderscoreIndex = eventName.LastIndexOf('_');

        if (lastUnderscoreIndex != -1)
        {
            var eventType = eventName.Substring(0, lastUnderscoreIndex);
            var subType = eventName.Substring(lastUnderscoreIndex + 1);

            if (string.Equals(subType, "delta", StringComparison.Ordinal) || string.Equals(subType, "start", StringComparison.Ordinal) || string.Equals(subType, "stop", StringComparison.Ordinal))
            {
                return (eventType, subType);
            }
        }

        return (eventName, null);
    }
#endif
}

internal class StreamPartialResponse
{
    public class StreamArrayItemBase : TypeBaseResponse
    {
        [JsonPropertyName("index")]
        public int? Index { get; set; }

        [JsonPropertyName("usage")]
        public Usage? Usage { get; set; }
    }

    public class DeltaItem<T> : StreamArrayItemBase
    {
        [JsonPropertyName("delta")]
        public T? Delta { get; set; }
    }

    public class InputJsonDelta
    {
        [JsonPropertyName("partial_json")]
        public string? PartialJson { get; set; }
    }

    public class MessageEventItem : TypeBaseResponse
    {
        [JsonPropertyName("message")]
        public MessageResponse? Message { get; set; }
    }

    public class ContentBlockItem : StreamArrayItemBase
    {
        [JsonPropertyName("content_block")]
        public ContentBlock? ContentBlock { get; set; }
    }
}

internal class StreamToolUseJsonBuilder
{
    private readonly HashSet<int?> _activeIndices = [];
    private readonly Dictionary<int?, (ContentBlock InitialBlock, StringBuilder JsonBuilder)> _blocks = [];

    public void StartBlock(int? index, ContentBlock initialBlock)
    {
        index ??= int.MaxValue;
        _blocks[index] = (initialBlock, new());
        _activeIndices.Add(index);
    }

    public void AppendJson(int? index, string? partialJson)
    {
        index ??= int.MaxValue;
        if (_blocks.TryGetValue(index, out var block))
        {
            block.JsonBuilder.Append(partialJson);
        }
    }

    public ContentBlock FinishBlock(int? index)
    {
        index ??= int.MaxValue;
        if (_blocks.TryGetValue(index, out var block))
        {
            _activeIndices.Remove(index);
            _blocks.Remove(index);
            var finalJson = block.JsonBuilder.ToString();
            var inputJson = string.IsNullOrEmpty(finalJson) ? block.InitialBlock.Input : JsonSerializer.Deserialize<JsonElement>(finalJson);
            return new()
            {
                Type = block.InitialBlock.Type,
                Id = block.InitialBlock.Id,
                Name = block.InitialBlock.Name,
                Input = inputJson
            };
        }

        return new();
    }

    public bool IsActiveBlock(int? index)
    {
        index ??= int.MaxValue;
        return _activeIndices.Contains(index);
    }
}