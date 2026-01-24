using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CtaClient.Extensions;
using CtaClient.Interfaces;

namespace CtaClient.Tests.Extensions;

public class CtaClientExtensionsTests
{
    [Fact]
    public void ClientExtension_RegistersServices()
    {
        var inMemorySettings = new Dictionary<string, string?> {
            {"CtaApiSettings:ApiKey", "fakeKey"},
            {"CtaApiSettings:BaseAddress", "https://lapi/bogus.net"},
        };

        IConfiguration configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings.AsEnumerable())
            .Build();

        var services = new ServiceCollection();
        services.AddCtaClient(configuration);
        var serviceProvider = services.BuildServiceProvider();

        var httpClient = serviceProvider.GetRequiredService<HttpClient>();
        var endpointFactory = serviceProvider.GetRequiredService<CtaEndpointFactory>();
        var ctaClient = serviceProvider.GetRequiredService<ICtaClient>();

        Assert.NotNull(httpClient);
        Assert.NotNull(endpointFactory);
        Assert.NotNull(ctaClient);
    }
}
