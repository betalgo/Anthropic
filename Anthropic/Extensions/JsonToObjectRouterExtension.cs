using System.Text.Json;
using Anthropic.ObjectModels;

namespace Anthropic.Extensions;

internal static class JsonToObjectRouterExtension
{
    public static Type Route(string json)
    {
        var apiResponse = JsonSerializer.Deserialize<TypeBaseResponse>(json);

        return apiResponse?.Type switch
        {
            "message_start" => typeof(MessageResponse),
            "message_delta" => typeof(MessageResponse),
            "message_stop" => typeof(MessageResponse),
            "content_block_start" => typeof(MessageResponse),
            "content_block_delta" => typeof(MessageResponse),
            "content_block_stop" => typeof(MessageResponse),
            "ping" => typeof(MessageResponse),
            _ => typeof(BaseResponse)
        };
    }
}