using Anthropic.Extensions;
using Anthropic.ObjectModels.ResponseModels;
using Anthropic.ObjectModels.SharedModels;
using Anthropic.Playground.SampleModels;
using Anthropic.Services;

namespace Anthropic.Playground.TestHelpers;

/// <summary>
///     Helper class for testing chat functionalities with tool use in the Anthropic SDK.
/// </summary>
internal static class ChatToolUseTestHelper
{
    /// <summary>
    ///     Runs a chat completion stream test with tool use using the Anthropic SDK.
    /// </summary>
    /// <param name="anthropicService">The Anthropic service instance.</param>
    public static async Task RunChatCompletionWithToolUseTest(IAnthropicService anthropicService)
    {
        Console.WriteLine("Starting Chat Completion Stream Test with Tool Use:");
        try
        {
            Console.WriteLine("Initiating chat completion stream...");
            var chatCompletionStream = anthropicService.Messages.CreateAsStream(new()
            {
                Messages = [Message.FromUser("What is the weather like in San Francisco?")],
                MaxTokens = 2024,
                Model = "claude-3-5-sonnet-20240620",
                Tools =
                [
                    new()
                    {
                        Name = "get_weather",
                        Description = "Get the current weather in a given location",
                        InputSchema = PropertyDefinition.DefineObject(new Dictionary<string, PropertyDefinition>
                        {
                            { "location", PropertyDefinition.DefineString("The city and state, e.g. San Francisco, CA") }
                        }, ["location"], null, null, null)
                    }
                ]
                // ToolChoice = new() { Type = "any" }
            });

            await foreach (var streamResponse in chatCompletionStream)
            {
                if (streamResponse.IsMessageResponse())
                {
                    var messageResponse = streamResponse.As<MessageResponse>();
                    var content = messageResponse.Content?.FirstOrDefault();
                    if (content == null)
                    {
                        continue;
                    }

                    switch (content.Type)
                    {
                        case "text":
                            Console.Write(content.Text);
                            break;
                        case "tool_use" when content.Name == "get_weather":
                            if (content.TryGetInputAs<WeatherInput>(out var weatherInput))
                            {
                                Console.WriteLine($"\nWeather request for \"{weatherInput.Location}\" in \"{weatherInput.Unit}\"");
                            }

                            break;
                        case "tool_use":
                            throw new InvalidOperationException($"Unknown tool use: {content.Name}");
                        default:
                            Console.WriteLine($"\nUnknown content type: {content.Type}");
                            break;
                    }
                }
                else if (streamResponse.IsError())
                {
                    var errorResponse = streamResponse.As<BaseResponse>();
                    Console.WriteLine($"Error: {errorResponse.HttpStatusCode}: {errorResponse.Error?.Message}");
                }
                else if (streamResponse.IsPingResponse())
                {
                    // Optionally handle ping responses
                    // Console.WriteLine("Ping received");
                }
            }

            Console.WriteLine("\nStream completed");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex}");
            throw;
        }

        Console.WriteLine("----  0  ----");
    }
}