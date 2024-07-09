using System;

namespace Anthropic.Services;

public class AnthropicOptions
{
    private const string AnthropicDefaultApiVersion = "v1";
    private const string AnthropicDefaultBaseDomain = "https://api.anthropic.com/";


    /// <summary>
    ///     Setting key for Json Setting Bindings
    /// </summary>
    public static readonly string SettingKey = "AnthropicServiceOptions";
    
    private string? _apiVersion;
    private string? _baseDomain;


    public ProviderType ProviderType { get; set; } = ProviderType.Anthropic;

    public string ApiKey { get; set; } = null!;

    public string ApiVersion
    {
        get
        {
            return _apiVersion ??= ProviderType switch
            {
                ProviderType.Anthropic => AnthropicDefaultApiVersion,
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
                ProviderType.Anthropic => AnthropicDefaultBaseDomain,
                _ => throw new ArgumentOutOfRangeException(nameof(ProviderType))
            };
        }
        set => _baseDomain = value;
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