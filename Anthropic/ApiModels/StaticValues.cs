namespace Betalgo.Anthropic.ApiModels;

internal class StaticValues
{
    public static class TypeConstants
    {
        public const string Message = "message";
        public const string ContentBlock = "content_block";
        public const string Text = "text";
        public const string ToolUse = "tool_use";
        public const string InputJson = "input_json";
        public const string Unknown = "unknown";
    }

    public static class StreamConstants
    {
        public const string Event = "event: ";
        public const int EventCharCount = 7;
        public const string Data = "data: ";
        public const int DataCharCount = 6;
    }

    public static class EventSubTypes
    {
        public const string Ping = "ping";
        public const string Start = "start";
        public const string Delta = "delta";
        public const string Stop = "stop";
    }
}