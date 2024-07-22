using Anthropic.ApiModels;
using Anthropic.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Anthropic.Extensions;

public static class AnthropicServiceCollectionExtensions
{
    public static IHttpClientBuilder AddAnthropicService(this IServiceCollection services, Action<AnthropicOptions>? setupAction = null)
    {
        var optionsBuilder = services.AddOptions<AnthropicOptions>();
        optionsBuilder.BindConfiguration(AnthropicOptions.SettingKey);
        if (setupAction != null)
        {
            optionsBuilder.Configure(setupAction);
        }

        return services.AddHttpClient<IAnthropicService, AnthropicService>();
    }

    public static IHttpClientBuilder AddAnthropicService<TServiceInterface>(this IServiceCollection services, string name, Action<AnthropicOptions>? setupAction = null) where TServiceInterface : class, IAnthropicService
    {
        var optionsBuilder = services.AddOptions<AnthropicOptions>(name);
        optionsBuilder.BindConfiguration($"{AnthropicOptions.SettingKey}:{name}");
        if (setupAction != null)
        {
            optionsBuilder.Configure(setupAction);
        }

        return services.AddHttpClient<TServiceInterface>();
    }
}