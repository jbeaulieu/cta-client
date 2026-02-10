using Microsoft.Extensions.Options;
using CtaClient.Config;
using CtaClient.Models;
using CtaClient.Exceptions;

namespace CtaClient.Tests;

public class CtaEndpointFactoryTests
{
    private const string MOCK_API_KEY = "fakeKey";
    private const string MOCK_BASE_ADDR = "https://lapi/bogus.net";
    private const int MAP_ID_1 = 12345;
    private const int MAP_ID_2 = 54321;
    private const int STOP_ID_1 = 67890;
    private const int STOP_ID_2 = 09876;
    private const Route ROUTE_1 = Route.Purple;
    private const Route ROUTE_2 = Route.Red;
    private const int maxResults = 8;
    private readonly string baseExpectedResponse;
    private readonly CtaEndpointFactory factory;

    public CtaEndpointFactoryTests()
    {
        IOptions<CtaApiSettings> mockApiSettings = Options.Create(new CtaApiSettings
        {
            ApiKey = MOCK_API_KEY,
            BaseAddress = MOCK_BASE_ADDR,
        });

        factory = new CtaEndpointFactory(mockApiSettings);

        baseExpectedResponse = $"{MOCK_BASE_ADDR}?key={MOCK_API_KEY}&outputType=JSON";
    }

    #region Constructor Tests

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CtaEndpointFactory_MissingApiKey_Throws(string? apiKey)
    {
        var misconfiguredOptions = Options.Create(new CtaApiSettings{ ApiKey = apiKey!, BaseAddress = MOCK_BASE_ADDR });

        Assert.Throws<ArgumentNullException>(() => new CtaEndpointFactory(misconfiguredOptions));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CtaEndpointFactory_MissingBaseAddress_Throws(string? baseAddress)
    {
        var misconfiguredOptions = Options.Create(new CtaApiSettings{ ApiKey = MOCK_API_KEY, BaseAddress = baseAddress! });

        Assert.Throws<ArgumentNullException>(() => new CtaEndpointFactory(misconfiguredOptions));
    }

    #endregion

    #region GetArrivalsEndpoint Tests

    [Fact]
    public void CtaEndpointFactory_GetArrivals_MissingStopIdAndMapId_Throws()
    {
        var request = new ArrivalsRequest
        {
            Routes = [ROUTE_1],
            MaxResults = maxResults
        };

        Assert.Throws<MissingParameterException>(() => factory.GetArrvialsEndpoint(request));
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsMapId()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={MAP_ID_1}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsMultipleMapIds()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1, MAP_ID_2],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={MAP_ID_1}&mapid={MAP_ID_2}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsMapId()
    {
        var request = new ArrivalsRequest
        {
            StopIds = [STOP_ID_1],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        Assert.DoesNotContain("mapid", result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsStopId()
    {
        var request = new ArrivalsRequest
        {
            StopIds = [STOP_ID_1],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&stpid={STOP_ID_1}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsMultipleStopIds()
    {
        var request = new ArrivalsRequest
        {
            StopIds = [STOP_ID_1, STOP_ID_2],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&stpid={STOP_ID_1}&stpid={STOP_ID_2}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsStopId()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        Assert.DoesNotContain("stpid", result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsRoute()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
            Routes = [ROUTE_1],
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={MAP_ID_1}&rt={ROUTE_1.GetServiceId()}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsMultipleRoutes()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
            Routes = [ROUTE_1, ROUTE_2],
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={MAP_ID_1}&rt={ROUTE_1.GetServiceId()}&rt={ROUTE_2.GetServiceId()}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsRoute()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
            StopIds = [STOP_ID_2],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        Assert.DoesNotContain("rt", result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_AddsMaxResults()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_2],
            StopIds = [STOP_ID_1],
            MaxResults = maxResults
        };

        var result = factory.GetArrvialsEndpoint(request);

        var expected = $"{baseExpectedResponse}&mapid={MAP_ID_2}&stpid={STOP_ID_1}&max={maxResults}";

        Assert.Equal(expected, result.ToString());
    }

    [Fact]
    public void CtaEndpointFactory_GetArrivals_OmitsMaxResults()
    {
        var request = new ArrivalsRequest
        {
            MapIds = [MAP_ID_1],
        };

        var result = factory.GetArrvialsEndpoint(request);

        Assert.DoesNotContain("max", result.ToString());
    }

    #endregion
}
