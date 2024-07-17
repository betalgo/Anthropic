namespace Anthropic.EndpointProviders;

public class AnthropicEndpointProvider : IAnthropicEndpointProvider
{
    private readonly string _apiVersion;

    public AnthropicEndpointProvider(string apiVersion)
    {
        _apiVersion = apiVersion;
    }

    public string CreateMessage()
    {
        return EndpointWithApiVersion("messages");
    }

    private string EndpointWithApiVersion(string endpoint)
    {
        return $"{_apiVersion}/{endpoint}";
    }
}