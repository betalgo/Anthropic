using Anthropic.Extensions;
using Anthropic.ObjectModels.RequestModels;
using Anthropic.Playground.TestHelpers;
using Anthropic.Services;
using LaserCatEyes.HttpClientListener;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


var builder = new ConfigurationBuilder().AddUserSecrets<Program>();

IConfiguration configuration = builder.Build();
var serviceCollection = new ServiceCollection();
serviceCollection.AddScoped(_ => configuration);

// Laser cat eyes is a tool that shows your requests and responses between Anthropic server and your client.
// Get your app key from https://lasercateyes.com for FREE and put it under ApiSettings.json or secrets.json.
// It is in Beta version, if you don't want to use it just comment out below line.
serviceCollection.AddLaserCatEyesHttpClientListener();

//if you want to use beta services you have to set UseBeta to true. Otherwise, it will use the stable version of Anthropic apis.
serviceCollection.AddAnthropicService();

//serviceCollection.AddAnthropicService(options =>
//{
//    options.ApiKey = "MyApiKey";
//});

var serviceProvider = serviceCollection.BuildServiceProvider();
var sdk = serviceProvider.GetRequiredService<IAnthropicService>();

await ChatTestHelper.RunChatCompletionTest(sdk);
await ChatTestHelper.RunChatCompletionStreamTest(sdk);
await ChatToolUseTestHelper.RunChatCompletionWithToolUseTest(sdk);
Console.WriteLine("Press any key to exit...");
Console.ReadLine();