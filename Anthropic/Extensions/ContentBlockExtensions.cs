using System.Text.Json;
using Betalgo.Anthropic.ApiModels.SharedModels;

namespace Betalgo.Anthropic.Extensions;

public static class ContentBlockExtensions
{
    public static T? GetInputAs<T>(this ContentBlock contentBlock)
    {
        if (contentBlock.Input is JsonElement jsonElement)
        {
            return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
        }

        return default;
    }

    public static bool TryGetInputAs<T>(this ContentBlock contentBlock, out T? result)
    {
        result = default;
        if (contentBlock.Input is JsonElement jsonElement)
        {
            try
            {
                result = JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }
}