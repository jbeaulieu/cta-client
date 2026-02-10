using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using System.Net;
using System.Text.Json;
using CtaClient.Config;
using CtaClient.Models;

namespace CtaClient.Tests;

public class CtaClientTests
{
    private readonly Mock<ILogger<CtaClient>> mockLogger;
    private readonly Mock<CtaEndpointFactory> mockEndpointFactory;
    private readonly CtaApiResult response;
    public CtaClientTests()
    {
        mockLogger = new Mock<ILogger<CtaClient>>();

        var fakeApiSettings = new OptionsWrapper<CtaApiSettings>(new CtaApiSettings
        {
            ApiKey = "fakeKey",
            BaseAddress = "https://lapi/bogus.net",
        });

        mockEndpointFactory = new Mock<CtaEndpointFactory>(fakeApiSettings);

        var arrivalsResponse = new ArrivalsResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = 0,
            Arrivals = [ new() { Route = Route.Red }],
        };

        response = new CtaApiResult { Response = arrivalsResponse };
    }

    private Mock<CtaClient> CreateCtaClient(Mock<HttpMessageHandler> httpMessageHandler)
    {
        return new Mock<CtaClient>(httpMessageHandler.CreateClient(), mockEndpointFactory.Object, mockLogger.Object)
            { CallBase = true };
    }

    [Fact]
    public async Task Client_GetArrivals_Runs()
    {
        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(response))
                });

        var request = new ArrivalsRequest
        {
            MapIds = [12345],
        };

        var client = CreateCtaClient(httpMessageHandler);
        var result = await client.Object.GetArrivals(request);

        Assert.Single(httpMessageHandler.Invocations);
        Assert.IsType<ArrivalsResponse>(result);
        Assert.Single(result.Arrivals);
        Assert.Equal(Route.Red, result.Arrivals[0].Route);
    }
}
