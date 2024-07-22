using Anthropic.ApiModels;
using Anthropic.EndpointProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Anthropic.Services;

public partial class AnthropicService : IAnthropicService, IDisposable
{
    private readonly bool _disposeHttpClient;
    private readonly IAnthropicEndpointProvider _endpointProvider;
    private readonly HttpClient _httpClient;
    private string? _defaultModelId;

    [ActivatorUtilitiesConstructor]
    public AnthropicService(IOptions<AnthropicOptions> settings, HttpClient httpClient) : this(settings.Value, httpClient)
    {
    }

    public AnthropicService(AnthropicOptions settings, HttpClient? httpClient = null)
    {
        settings.Validate();

        if (httpClient == null)
        {
            _disposeHttpClient = true;
            _httpClient = new();
        }
        else
        {
            _httpClient = httpClient;
        }

        _httpClient.BaseAddress = new(settings.BaseDomain);
        _httpClient.DefaultRequestHeaders.Add("anthropic-version", settings.ProviderVersion);
        switch (settings.ProviderType)
        {
            case AnthropicProviderType.Anthropic:
                _httpClient.DefaultRequestHeaders.Add("x-api-key", settings.ApiKey);
                break;
        }

        _endpointProvider = settings.ProviderType switch
        {
            AnthropicProviderType.Anthropic => new AnthropicEndpointProvider(settings.ApiVersion),
            _ => throw new ArgumentOutOfRangeException(nameof(settings.ProviderType))
        };

        _defaultModelId = settings.DefaultModelId;
    }


    /// <inheritdoc />
    public void SetDefaultModelId(string modelId)
    {
        _defaultModelId = modelId;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_disposeHttpClient && _httpClient != null)
            {
                _httpClient.Dispose();
            }
        }
    }
}