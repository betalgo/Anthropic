namespace Anthropic.EndpointProviders;

public class AnthropicEndpointProvider : IAnthropicEndpointProvider
{
    public string CreateMessage()
    {
        return EndpointWithApiVersion("messages");
    }

    private static string EndpointWithApiVersion(string endpoint)
    {
        return $"v1/{endpoint}";
    }
}