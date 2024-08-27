namespace Betalgo.Anthropic.ApiModels;

public class AnthropicOptions
{
    private const string AnthropicDefaultApiVersion = "v1";
    private const string AnthropicDefaultVersion = "2023-06-01";
    private const string AnthropicDefaultBaseDomain = "https://api.anthropic.com/";


    /// <summary>
    ///     Setting key for Json Setting Bindings
    /// </summary>
    public static readonly string SettingKey = "AnthropicServiceOptions";

    private string? _apiVersion;
    private string? _baseDomain;

    private string? _providerVersion;


    public AnthropicProviderType ProviderType { get; set; } = AnthropicProviderType.Anthropic;

    public string ApiKey { get; set; } = null!;

    public string ApiVersion
    {
        get
        {
            return _apiVersion ??= ProviderType switch
            {
                AnthropicProviderType.Anthropic => AnthropicDefaultApiVersion,
                _ => throw new ArgumentOutOfRangeException(nameof(ProviderType))
            };
        }
        set => _apiVersion = value;
    }

    /// <summary>
    ///     Base Domain
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public string BaseDomain
    {
        get
        {
            return _baseDomain ??= ProviderType switch
            {
                AnthropicProviderType.Anthropic => AnthropicDefaultBaseDomain,
                _ => throw new ArgumentOutOfRangeException(nameof(ProviderType))
            };
        }
        set => _baseDomain = value;
    }

    public string ProviderVersion
    {
        get
        {
            return _providerVersion ??= ProviderType switch
            {
                AnthropicProviderType.Anthropic => AnthropicDefaultVersion,
                _ => throw new ArgumentOutOfRangeException(nameof(ProviderType))
            };
        }
        set => _providerVersion = value;
    }

    public bool ValidateApiOptions { get; set; } = true;

    /// <summary>
    ///     Default model id. If you are working with only one model, this will save you from few line extra code.
    /// </summary>
    public string? DefaultModelId { get; set; }


    /// <summary>
    ///     Validate Settings
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    public void Validate()
    {
        if (!ValidateApiOptions)
        {
            return;
        }

        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentNullException(nameof(ApiKey));
        }

        if (string.IsNullOrEmpty(ApiVersion))
        {
            throw new ArgumentNullException(nameof(ApiVersion));
        }

        if (string.IsNullOrEmpty(BaseDomain))
        {
            throw new ArgumentNullException(nameof(BaseDomain));
        }
    }
}