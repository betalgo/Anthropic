using System.Text.Json.Serialization;

namespace Anthropic.ObjectModels.SharedModels;

public class Message
{
    public Message()
    {
    }

    private Message(string assistant, string content)
    {
        Role = assistant;
        Content = [ContentBlock.CreateText(content)];
    }

    private Message(string assistant, List<ContentBlock> contents)
    {
        Role = assistant;
        Content = [.. contents];
    }

    [JsonPropertyName("role")]
    public string Role { get; set; }

    [JsonPropertyName("content")]
    public List<ContentBlock> Content { get; set; }


    public static Message FromAssistant(string content)
    {
        return new("assistant", content);
    }

    public static Message FromUser(string content)
    {
        return new("user", content);
    }

    public static Message FromUser(List<ContentBlock> contents)
    {
        return new("user", contents);
    }
}