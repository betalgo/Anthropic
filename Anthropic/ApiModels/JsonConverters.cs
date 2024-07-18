using System.Text.Json;
using System.Text.Json.Serialization;
using Anthropic.ObjectModels.SharedModels;

namespace Anthropic.ObjectModels;

internal class JsonConverters
{
    public class ContentConverter : JsonConverter<List<ContentBlock>>
    {
        public override List<ContentBlock> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Expected start of array");
            }

            var content = new List<ContentBlock>();

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.EndArray:
                        return content;
                    case JsonTokenType.StartObject:
                        content.Add(JsonSerializer.Deserialize<ContentBlock>(ref reader, options)!);
                        break;
                    case JsonTokenType.String:
                        // Handle the case where content is a single string
                        content.Add(new() { Type = "text", Text = reader.GetString() });
                        return content; // Return immediately as this is a single-item array
                }
            }

            throw new JsonException("Expected end of array");
        }

        public override void Write(Utf8JsonWriter writer, List<ContentBlock> value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal class InputJsonConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);
            return doc.RootElement.Clone();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}