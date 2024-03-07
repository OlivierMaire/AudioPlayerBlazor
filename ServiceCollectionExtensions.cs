using AudioPlayerBlazor.Services;
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
        services.AddScoped<PlayerInterop>();
        services.AddScoped<MediaPlayerService>();

        switch (serviceLifetime)
        {
            case ServiceLifetime.Singleton:
                services.AddSingleton<QueuePlayerService>();
                services.AddSingleton<DisplayService>();
                break;
            case ServiceLifetime.Scoped:
                services.AddScoped<QueuePlayerService>();
                services.AddScoped<DisplayService>();
                break;
            case ServiceLifetime.Transient:
            default:
                services.AddTransient<QueuePlayerService>();
                services.AddTransient<DisplayService>();
                break;
        }

        services.Configure(optionsBuilder);

        return services;
    }
}