![Betalgo Ranul Anthropic Github readme banner](https://github.com/user-attachments/assets/c669c7b0-4e37-4c8d-b8fb-b3dd13a8f5ed)
# .NET Library for Anthropic Claude

⭐ We appreciate your star, it helps! ![GitHub Repo stars](https://img.shields.io/github/stars/betalgo/Anthropic)  
 #### Community Links
 We have a very new [![Discord Shield](https://discord.com/api/guilds/1250841506785529916/widget.png?style=shield)](https://discord.gg/gfgHsWnGxy)   and [![Static Badge](https://img.shields.io/badge/Reddit-BetalgoDevelopers-orange)](https://www.reddit.com/r/BetalgoDevelopers) channel. Please come and help us build the .NET AI community.
 

This C# library is created by [Betalgo](https://github.com/betalgo) for the Ranul Tinga Project and is released under the MIT license.

## Overview
A simple and efficient .NET library for accessing Anthropic's Claude AI API. This community-provided library allows you to easily integrate Claude's powerful AI capabilities into your C# applications.

### Install Package
[![Betalgo.Ranul.Anthropic](https://img.shields.io/nuget/v/Betalgo.Ranul.Anthropic?style=for-the-badge)](https://www.nuget.org/packages/Betalgo.Ranul.Anthropic/)
```shell
Install-Package Betalgo.Ranul.Anthropic
```

## Documentation and Examples
- [Wiki Page](https://github.com/betalgo/anthropic/wiki)
- [Playground Project](https://github.com/betalgo/Anthropic/tree/master/Anthropic.Playground)

The repository contains a sample project named **Anthropic.Playground** to help you understand how to work with Claude using this library. Please refer to the Wiki for detailed documentation and advanced usage scenarios.

## Quick Start

Here's a simple example demonstrating how easy it is to use Claude with the Anthropic .NET Library:

```csharp
var anthropicService = new AnthropicService(new()
{
    ApiKey = Environment.GetEnvironmentVariable("MY_ANTHROPIC_API_KEY")
});

var messageRequest = new MessageRequest
{
    Messages = [Message.FromUser("What is the capital of France?")],
    Model = "claude-3-opus-20240229",
    MaxTokens = 100
};

var messageResponse = await anthropicService.Messages.Create(messageRequest);
if (messageResponse.Successful)
{
    Console.WriteLine(messageResponse.ToString());
}
```

## Notes
This library is a community project for interacting with Anthropic's Claude AI and is not officially supported by Anthropic. Please use it responsibly and in accordance with Anthropic's usage policies for Claude.

## Changelog
### 8.0.0
- Initial release of the Anthropic Claude .NET Library

---

## Acknowledgements
Maintenance of this project is made possible by all the bug reporters, [contributors](https://github.com/betalgo/anthropic/graphs/contributors), and [sponsors](https://github.com/sponsors/kayhantolga).

For any issues, contributions, or feedback related to using this library with Claude, feel free to reach out or submit a pull request.

Betalgo GitHub: [https://github.com/betalgo](https://github.com/betalgo)  
Betalgo Twitter: [@Betalgo](https://twitter.com/Betalgo)  
Betalgo LinkedIn: [Betalgo | LinkedIn](https://www.linkedin.com/company/betalgo-up)
