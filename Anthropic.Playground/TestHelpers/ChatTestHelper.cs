using Anthropic.ObjectModels;
using Anthropic.Services;

namespace Anthropic.Playground.TestHelpers;

internal static class ChatTestHelper
{
    public static async Task RunSimpleChatTest(IAnthropicService sdk)
    {
        Console.WriteLine("Messsaging Testing is starting:");

        try
        {
            Console.WriteLine("Chat Completion Test:", ConsoleColor.DarkCyan);
            var completionResult = await sdk.Messages.Create(new()
            {
                Messages = new List<Message>
                {
                    Message.FromUser("Hello there."),
                    Message.FromAssistant("Hi, I'm Claude. How can I help you?"),
                    Message.FromUser("Can you explain LLMs in plain English?")
                },
                MaxTokens = 50,
                Model = "claude-3-5-sonnet-20240620"
            });

            if (completionResult.Successful)
            {
                Console.WriteLine(completionResult.Content.First().Text);
            }
            else
            {
                if (completionResult.Error == null)
                {
                    throw new("Unknown Error");
                }

                Console.WriteLine($"{completionResult.HttpStatusCode}: {completionResult.Error.Message}");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public static async Task RunSimpleCompletionStreamTest(IAnthropicService sdk)
    {
        Console.WriteLine("Chat Completion Stream Testing is starting:", ConsoleColor.Cyan);
        try
        {
            Console.WriteLine("Chat Completion Stream Test:", ConsoleColor.DarkCyan);
            var completionResult = sdk.Messages.CreateAsStream(new()
            {
                Messages = new List<Message>
                {
                    Message.FromUser("Hello there."),
                    Message.FromAssistant("Hi, I'm Claude. How can I help you?"),
                    Message.FromUser("Tell me really long story about how a purple color became human?(in Turkish)")
                },
                MaxTokens = 1024,
                Model = "claude-3-5-sonnet-20240620"
            });

            await foreach (var completion in completionResult)
            {
                if (completion.Successful)
                {
                    Console.Write(completion?.Content?.First().Text);
                }
                else
                {
                    if (completion.Error == null)
                    {
                        throw new("Unknown Error");
                    }

                    Console.WriteLine($"{completion.HttpStatusCode}: {completion.Error.Message}");
                }
            }

            Console.WriteLine("");
            Console.WriteLine("Complete");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
