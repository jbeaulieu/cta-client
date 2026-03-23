using System.Net;
using System.Net.Http.Json;
using CtaClient.Exceptions;
using CtaClient.Extensions;
using CtaClient.Models;

namespace CtaClient.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
    private readonly ArrivalsResponse arrivalsResponse;
    private readonly FollowTrainResponse followTrainResponse;

    public HttpResponseMessageExtensionsTests()
    {
        arrivalsResponse = new ArrivalsResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            ErrorDescription = null,
            Arrivals = []
        };

        followTrainResponse = new FollowTrainResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            ErrorDescription = null,
            Position = new(),
            Arrivals = []
        };
    }

    [Fact]
    public async Task HandleCtaApiResponse_Arrivals_Runs()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { Response = arrivalsResponse })
        };

        var result = await responseMessage.HandleCtaApiResponse<ArrivalsResponse>();

        Assert.IsType<Result<ArrivalsResponse>>(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task HandleCtaApiResponse_FollowThisTrain_Runs()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { Response = followTrainResponse })
        };

        var result = await responseMessage.HandleCtaApiResponse<ArrivalsResponse>();

        Assert.IsType<Result<ArrivalsResponse>>(result);
        Assert.True(result.IsSuccess);
        Assert.Equal(Error.None, result.Error);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task HandleCtaApiResponse_InvalidJson_Throws()
    {
        string badJson = "{\"ctatt\":2}";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { Response = badJson })
        };

        var ex = await Assert.ThrowsAsync<CtaClientException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
        Assert.NotNull(ex.InnerException);
        Assert.NotNull(ex.Message);
    }

    #region HTTP Errors

    [Theory]
    [InlineData(HttpStatusCode.BadRequest)]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.RequestTimeout)]
    [InlineData(HttpStatusCode.NotImplemented)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task HandleCtaApiResponse_HttpError_Throws(HttpStatusCode statusCode)
    {
        var responseMessage = new HttpResponseMessage(statusCode)
        {
            Content = JsonContent.Create(new { Response = arrivalsResponse })
        };

        var ex = await Assert.ThrowsAsync<CtaClientException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
        Assert.IsType<HttpRequestException>(ex.InnerException);
        Assert.Equal(statusCode, ((HttpRequestException)ex.InnerException).StatusCode);
    }

    #endregion HTTP Errors

    #region CTA Errors

    [Theory]
    [InlineData(ErrorCode.MissingParameter)]
    [InlineData(ErrorCode.InvalidApiKey)]
    [InlineData(ErrorCode.DailyLimitExceeded)]
    [InlineData(ErrorCode.InvalidMapId)]
    [InlineData(ErrorCode.MapIdNotInteger)]
    [InlineData(ErrorCode.MaxMapIdsExceeded)]
    [InlineData(ErrorCode.InvalidRoute)]
    [InlineData(ErrorCode.MaxRoutesExceeded)]
    [InlineData(ErrorCode.InvalidStopId)]
    [InlineData(ErrorCode.MaxStopIdsExceeded)]
    [InlineData(ErrorCode.InvalidMaxParam)]
    [InlineData(ErrorCode.NonPositiveMaxParam)]
    [InlineData(ErrorCode.StopIdNotInteger)]
    [InlineData(ErrorCode.InvalidParameter)]
    [InlineData(ErrorCode.TrainNotFound)]
    [InlineData(ErrorCode.StopsUnavailable)]
    [InlineData(ErrorCode.PredictionsUnavailable)]
    [InlineData(ErrorCode.ServerError)]
    public async Task HandleCtaApiResponse_WithErrorCode_Runs(ErrorCode errorCode)
    {
        arrivalsResponse.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new { Response = arrivalsResponse })
        };

        var result = await responseMessage.HandleCtaApiResponse<ArrivalsResponse>();

        Assert.IsType<Result<ArrivalsResponse>>(result);
        Assert.True(result.IsFailure);
        Assert.Equal(errorCode, result.Error.Code);
        Assert.Null(result.Value);
    }

    #endregion CTA Errors
}
