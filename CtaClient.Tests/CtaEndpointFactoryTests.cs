using Microsoft.Extensions.Options;
using CtaClient.Config;
using CtaClient.Models;

namespace CtaClient.Tests;

public class CtaEndpointFactoryTests
{
    private readonly string mockApiKey = "fakeKey";
    private readonly string mockBaseAddress = "https://lapi/bogus.net";
    private readonly int mapId = 12345;
    private readonly int stopId = 67890;
    private readonly Route route = Route.Purple;
    private readonly int maxResults = 8;
    private readonly string baseExpectedResponse;
    private readonly CtaEndpointFactory factory;

    public CtaEndpointFactoryTests()
    {
        IOptions<CtaApiSettings> mockApiSettings = Options.Create(new CtaApiSettings
        {
            ApiKey = mockApiKey,
            BaseAddress = mockBaseAddress,
        });

        factory = new CtaEndpointFactory(mockApiSettings);

        baseExpectedResponse = $"{mockBaseAddress}?key={mockApiKey}&outputType=JSON";
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CtaEndpointFactory_MissingApiKey_Throws(string? apiKey)
    {
        var misconfiguredOptions = Options.Create(new CtaApiSettings{ ApiKey = apiKey!, BaseAddress = mockBaseAddress });

        Assert.Throws<ArgumentNullException>(() => new CtaEndpointFactory(misconfiguredOptions));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CtaEndpointFactory_MissingBaseAddress_Throws(string? baseAddress)
    {
        var misconfiguredOptions = Options.Create(new CtaApiSettings{ ApiKey = mockApiKey, BaseAddress = baseAddress! });

        Assert.Throws<ArgumentNullException>(() => new CtaEndpointFactory(misconfiguredOptions));
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_MapId_OmitsStopIdParam()
    {
        var request = new ArrivalsRequest
        {
            MapId = mapId,
            Route = route,
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={mapId}&rt={route.ToServiceId()}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_StopId__OmitsMapIdParam()
    {
        var request = new ArrivalsRequest
        {
            StopId = stopId,
            Route = route,
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&stpid={stopId}&rt={route.ToServiceId()}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsRouteParam()
    {
        var request = new ArrivalsRequest
        {
            MapId = mapId,
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={mapId}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsMaxParam()
    {
        var request = new ArrivalsRequest
        {
            MapId = mapId,
            Route = route
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={mapId}&rt={route.ToServiceId()}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitStopIdAndMapId_Throws()
    {
        var request = new ArrivalsRequest
        {
            Route = route,
            MaxResults = maxResults
        };

        Assert.Throws<ArgumentException>(() => factory.GetArrvialsEndpoint(request));
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_StopIdAndMapId_Throws()
    {
        var request = new ArrivalsRequest
        {
            MapId = mapId,
            StopId = stopId,
            Route = route,
            MaxResults = maxResults
        };

        Assert.Throws<ArgumentException>(() => factory.GetArrvialsEndpoint(request));
    }
}
