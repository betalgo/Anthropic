using Anthropic.ObjectModels.ResponseModels;
using Anthropic.ObjectModels.SharedModels;
using Anthropic.Services;

namespace Anthropic.Playground.TestHelpers;

/// <summary>
///     Helper class for testing chat functionalities of the Anthropic SDK.
/// </summary>
internal static class ChatTestHelper
{
    /// <summary>
    ///     Runs a simple chat completion test using the Anthropic SDK.
    /// </summary>
    /// <param name="anthropicService">The Anthropic service instance.</param>
    public static async Task RunChatCompletionTest(IAnthropicService anthropicService)
    {
        Console.WriteLine("Starting Chat Completion Test:");
        try
        {
            Console.WriteLine("Sending chat completion request...");
            var chatCompletionResult = await anthropicService.Messages.Create(new()
            {
                Messages =
                [
                    Message.FromUser("Hello there."),
                    Message.FromAssistant("Hi, I'm Claude. How can I help you?"),
                    Message.FromUser("Can you explain LLMs in plain English?")
                ],
                MaxTokens = 200,
                Model = "claude-3-5-sonnet-20240620"
            });

            if (chatCompletionResult.Successful)
            {
                Console.WriteLine("Chat Completion Response:");
                Console.WriteLine(chatCompletionResult.ToString());
            }
            else
            {
                if (chatCompletionResult.Error == null)
                {
                    throw new("Unknown Error Occurred");
                }

                Console.WriteLine($"Error: {chatCompletionResult.HttpStatusCode}: {chatCompletionResult.Error.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex}");
            throw;
        }
        Console.WriteLine("----  0  ----");

    }

    /// <summary>
    ///     Runs a simple chat completion stream test using the Anthropic SDK.
    /// </summary>
    /// <param name="anthropicService">The Anthropic service instance.</param>
    public static async Task RunChatCompletionStreamTest(IAnthropicService anthropicService)
    {
        Console.WriteLine("Starting Chat Completion Stream Test:");
        try
        {
            Console.WriteLine("Initiating chat completion stream...");
            var chatCompletionStream = anthropicService.Messages.CreateAsStream(new()
            {
                Messages =
                [
                    Message.FromUser("Hello there."),
                    Message.FromAssistant("Hi, I'm Claude. How can I help you?"),
                    Message.FromUser("Tell me a really long story about how the color purple became human.")
                ],
                MaxTokens = 10,
                Model = "claude-3-5-sonnet-20240620"
            });

            await foreach (var streamResponse in chatCompletionStream)
            {
                if (streamResponse.IsMessageResponse())
                {
                    var messageCompletion = streamResponse.As<MessageResponse>();
                    Console.Write(messageCompletion.ToString());
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