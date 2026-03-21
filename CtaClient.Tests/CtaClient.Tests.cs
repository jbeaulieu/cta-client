using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Contrib.HttpClient;
using Moq.Protected;
using System.Net;
using System.Net.Http.Json;
using CtaClient.Config;
using CtaClient.Models;

namespace CtaClient.Tests;

public class CtaClientTests
{
    private readonly Mock<ILogger<CtaClient>> mockLogger;
    private readonly Mock<CtaEndpointFactory> mockEndpointFactory;
    public CtaClientTests()
    {
        mockLogger = new Mock<ILogger<CtaClient>>();

        var fakeApiSettings = new OptionsWrapper<CtaApiSettings>(new CtaApiSettings
        {
            ApiKey = "fakeKey",
            BaseAddress = "https://lapi.bogus/api/1.0",
        });

        mockEndpointFactory = new Mock<CtaEndpointFactory>(fakeApiSettings);
    }

    private Mock<CtaClient> CreateCtaClient(Mock<HttpMessageHandler> httpMessageHandler)
    {
        return new Mock<CtaClient>(httpMessageHandler.CreateClient(), mockEndpointFactory.Object, mockLogger.Object)
            { CallBase = true };
    }

    [Fact]
    public async Task Client_GetArrivals_Runs()
    {
        var arrivalsResponse = new ArrivalsResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            Arrivals = [ new() { Route = Route.Red }],
        };

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new { Response = arrivalsResponse })
                });

        var request = new ArrivalsRequest
        {
            MapIds = [12345],
        };

        var client = CreateCtaClient(httpMessageHandler);
        var result = await client.Object.GetArrivals(request);

        Assert.Single(httpMessageHandler.Invocations);

        Assert.IsType<Result<ArrivalsResponse>>(result);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Value!.Arrivals);
        Assert.Equal(Route.Red, result.Value!.Arrivals[0].Route);
    }

    [Fact]
    public async Task Client_FollowThisTrain_Runs()
    {
        var followTrainResponse = new FollowTrainResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            Position = new TrainPosition(),
            Arrivals = [ new() { Route = Route.Red }]
        };

        var httpMessageHandler = new Mock<HttpMessageHandler>();
        httpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = JsonContent.Create(new { Response = followTrainResponse })
                });

        var request = new FollowTrainRequest
        {
            RunNumber = 12345,
        };

        var client = CreateCtaClient(httpMessageHandler);
        var result = await client.Object.FollowThisTrain(request);

        Assert.Single(httpMessageHandler.Invocations);

        Assert.IsType<Result<FollowTrainResponse>>(result);
        Assert.Single(result.Value!.Arrivals);
        Assert.Equal(Route.Red, result.Value!.Arrivals[0].Route);
    }
}
