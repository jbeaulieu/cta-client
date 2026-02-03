using CtaClient.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CtaClient.Extensions;

/// <summary>
///   Extension methods for CTA Client service registration.
/// </summary>
public static class CtaClientExtensions
{
    /// <summary>
    ///   Extension method to register a <see cref="CtaClient" /> and corresponding endpoints specified
    ///   by <see cref="CtaEndpointFactory" /> in a <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection" /> to modify</param>
    /// <param name="config"><see cref="IConfiguration" /> to register for <see cref="CtaApiSettings" /> options</param>
    public static IServiceCollection AddCtaClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CtaApiSettings>(configuration.GetSection(nameof(CtaApiSettings)));

        services.AddHttpClient<ICtaClient, CtaClient>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(3));

        services.AddTransient<CtaEndpointFactory>();
        services.AddSingleton<ICtaClient, CtaClient>();
        return services;
    }
}
