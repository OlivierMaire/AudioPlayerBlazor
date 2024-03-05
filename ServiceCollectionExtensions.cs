using Microsoft.Extensions.DependencyInjection;

namespace AudioPlayerBlazor;

/// <summary>
/// Extension methods to setup the EPubBlazor services.
/// </summary>
public static class ServiceCollectionExtensions
{

    /// <summary>
    /// Add EPubBlazor services.
    /// </summary>
    /// <param name="services">The service collection to setup.</param>
    /// <param name="serviceLifetime">Service Lifetime to use to register the Services. (Default is Scoped)</param>
    /// <returns>The given service collection updated with the EPubBlazor services.</returns>
    public static IServiceCollection AddAudioPlayerBlazor(this IServiceCollection services, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        => services.AddAudioPlayerBlazor(_ => { }, serviceLifetime);

    /// <summary>
    /// Add EPubBlazor services.
    /// </summary>
    /// <param name="services">The service collection to setup.</param>
    /// <param name="optionsBuilder">Options builder action delegate.</param>
    /// <param name="serviceLifetime">Service Lifetime to use to register the Services. (Default is Scoped)</param>
    /// <returns>The given service collection updated with the EPubBlazor services.</returns>
    public static IServiceCollection AddAudioPlayerBlazor(this IServiceCollection services, 
        Action<AudioPlayerBlazorOptions> optionsBuilder, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
    {
        // services.AddJsBlob(serviceLifetime);
        // services.AddHttpClient();
        // services.AddScoped<EPubJsInterop>();

        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<AudioPlayerService>();
                break;
            case ServiceLifetime.Scoped:
                services.AddSingleton<AudioPlayerService>();
                break;
            case ServiceLifetime.Transient:
            default:
                services.AddSingleton<AudioPlayerService>();
                break;
        }

        services.Configure(optionsBuilder);

        return services;
    }
}