using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Exceptions;
using CtaClient.Extensions;
using CtaClient.Json;
using CtaClient.Models;

namespace CtaClient.Tests.Extensions;

public class HttpResponseMessageExtensionsTests
{
    private readonly CtaApiResult<ArrivalsResponse> arrivalsApiResult;
    private readonly CtaApiResult<FollowTrainResponse> followTrainApiResult;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false,
        Converters =
        {
            new JsonBoolConverter(),
            new JsonRouteConverter(),
            new JsonStringEnumConverter(),
        },
    };

    public HttpResponseMessageExtensionsTests()
    {
        var arrivalsResponse = new ArrivalsResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            ErrorDescription = null,
            Arrivals = []
        };

        arrivalsApiResult = new CtaApiResult<ArrivalsResponse>
        {
            Response = arrivalsResponse
        };

        var followTrainResponse = new FollowTrainResponse
        {
            Timestamp = DateTimeOffset.Now,
            ErrorCode = ErrorCode.None,
            ErrorDescription = null,
            Position = new(),
            Arrivals = []
        };

        followTrainApiResult = new CtaApiResult<FollowTrainResponse>
        {
            Response = followTrainResponse
        };
    }

    [Fact]
    public async Task HandleCtaApiResponse_Arrivals_Runs()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var result = await responseMessage.HandleCtaApiResponse<ArrivalsResponse>();

        Assert.IsType<ArrivalsResponse>(result);
    }

    [Fact]
    public async Task HandleCtaApiResponse_FollowThisTrain_Runs()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(followTrainApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var result = await responseMessage.HandleCtaApiResponse<ArrivalsResponse>();

        Assert.IsType<ArrivalsResponse>(result);
    }

    #region HTTP Errors

    [Fact]
    public async Task HandleCtaApiResponse_InvalidJson_Throws()
    {
        string badJson = "{\"ctatt\":2}";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(badJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<JsonException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_NotFound_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<NotFoundException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_RequestTimeout_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceTimedOutException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotImplemented)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task HandleCtaApiResponse_ServiceUnavailable_Throws(HttpStatusCode statusCode)
    {
        var responseMessage = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceUnavailableException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_OtherError_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    #endregion HTTP Errors

    #region CTA Errors

    [Fact]
    public async Task HandleCtaApiResponse_MissingParameter_Throws()
    {
        arrivalsApiResult.Response.ErrorCode = ErrorCode.MissingParameter;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<MissingParameterException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_InvalidApiKey_Throws()
    {
        arrivalsApiResult.Response.ErrorCode = ErrorCode.InvalidApiKey;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<InvalidApiKeyException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_LimitExceeded_Throws()
    {
        arrivalsApiResult.Response.ErrorCode = ErrorCode.DailyLimitExceeded;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<DailyLimitExceededException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Theory]
    [InlineData(ErrorCode.InvalidMapId)]
    [InlineData(ErrorCode.MapIdNotInteger)]
    [InlineData(ErrorCode.InvalidStopId)]
    [InlineData(ErrorCode.StopIdNotInteger)]
    [InlineData(ErrorCode.InvalidMaxParam)]
    [InlineData(ErrorCode.NonPositiveMaxParam)]
    [InlineData(ErrorCode.InvalidRoute)]
    [InlineData(ErrorCode.InvalidParameter)]
    [InlineData(ErrorCode.TrainNotFound)]
    public async Task HandleCtaApiResponse_InvalidParameter_Throws(ErrorCode errorCode)
    {
        arrivalsApiResult.Response.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<InvalidParameterException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Theory]
    [InlineData(ErrorCode.MaxMapIdsExceeded)]
    [InlineData(ErrorCode.MaxStopIdsExceeded)]
    [InlineData(ErrorCode.MaxRoutesExceeded)]
    public async Task HandleCtaApiResponse_MaxValuesExceeded_Throws(ErrorCode errorCode)
    {
        arrivalsApiResult.Response.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<MaxValuesExceededException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Theory]
    [InlineData(ErrorCode.StopsUnavailable)]
    [InlineData(ErrorCode.PredictionsUnavailable)]
    public async Task HandleCtaApiResponse_PredictionError_Throws(ErrorCode errorCode)
    {
        arrivalsApiResult.Response.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<PredictionException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    [Fact]
    public async Task HandleCtaApiResponse_ServerError_Throws()
    {
        arrivalsApiResult.Response.ErrorCode = ErrorCode.ServerError;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(arrivalsApiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServerErrorException>(responseMessage.HandleCtaApiResponse<ArrivalsResponse>);
    }

    #endregion CTA Errors
}
