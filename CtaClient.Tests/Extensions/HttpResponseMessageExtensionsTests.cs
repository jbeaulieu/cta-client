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
    private readonly CtaApiResult apiResult;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false,
        Converters =
        {
            new JsonBoolConverter(),
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

        apiResult = new CtaApiResult
        {
            Response = arrivalsResponse
        };
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_Runs()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        var result = await responseMessage.HandleCtaApiResponse();

        Assert.IsType<ArrivalsResponse>(result);
    }

    #region HTTP Errors

    [Fact]
    public async Task EnsureCtaApiSuccess_InvalidJson_Throws()
    {
        string badJson = "{\"ctatt\":2}";

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(badJson, Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<JsonException>(() => responseMessage.HandleCtaApiResponse());
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_NotFound_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<NotFoundException>(responseMessage.HandleCtaApiResponse);
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_RequestTimeout_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.RequestTimeout)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceTimedOutException>(responseMessage.HandleCtaApiResponse);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotImplemented)]
    [InlineData(HttpStatusCode.BadGateway)]
    [InlineData(HttpStatusCode.ServiceUnavailable)]
    [InlineData(HttpStatusCode.GatewayTimeout)]
    public async Task EnsureCtaApiSuccess_ServiceUnavailable_Throws(HttpStatusCode statusCode)
    {
        var responseMessage = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceUnavailableException>(responseMessage.HandleCtaApiResponse);
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_OtherError_Throws()
    {
        var responseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServiceException>(responseMessage.HandleCtaApiResponse);
    }

    #endregion HTTP Errors

    #region CTA Errors

    [Fact]
    public async Task EnsureCtaApiSuccess_MissingParameter_Throws()
    {
        apiResult.Response.ErrorCode = ErrorCode.MissingParameter;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<MissingParameterException>(responseMessage.HandleCtaApiResponse);
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_InvalidApiKey_Throws()
    {
        apiResult.Response.ErrorCode = ErrorCode.InvalidApiKey;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<InvalidApiKeyException>(responseMessage.HandleCtaApiResponse);
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_LimitExceeded_Throws()
    {
        apiResult.Response.ErrorCode = ErrorCode.DailyLimitExceeded;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<DailyLimitExceededException>(responseMessage.HandleCtaApiResponse);
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
    public async Task EnsureCtaApiSuccess_InvalidParameter_Throws(ErrorCode errorCode)
    {
        apiResult.Response.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<InvalidParameterException>(responseMessage.HandleCtaApiResponse);
    }

    [Theory]
    [InlineData(ErrorCode.MaxMapIdsExceeded)]
    [InlineData(ErrorCode.MaxStopIdsExceeded)]
    [InlineData(ErrorCode.MaxRoutesExceeded)]
    public async Task EnsureCtaApiSuccess_MaxValuesExceeded_Throws(ErrorCode errorCode)
    {
        apiResult.Response.ErrorCode = errorCode;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<MaxValuesExceededException>(responseMessage.HandleCtaApiResponse);
    }

    [Fact]
    public async Task EnsureCtaApiSuccess_ServerError_Throws()
    {
        apiResult.Response.ErrorCode = ErrorCode.ServerError;

        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(JsonSerializer.Serialize(apiResult, JsonOptions), Encoding.UTF8, MediaTypeNames.Application.Json)
        };

        await Assert.ThrowsAsync<ServerErrorException>(responseMessage.HandleCtaApiResponse);
    }

    #endregion CTA Errors
}
