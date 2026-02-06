using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Exceptions;
using CtaClient.Json;
using CtaClient.Models;

namespace CtaClient.Extensions;

internal static class HttpResponseMessageExtensions
{
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

    /// <summary>
    ///   Validates that the response from the CTA API indicates success, and extracts the response
    ///     model if so. If the response indicates an error, either from the response status code or
    ///     a CTA error code, a corresponding exception is thrown.
    /// </summary>
    /// <param name="response">The Http response message to validate</param>
    /// <returns>The validated ArrivalsResponse deserialized from the message response</returns>
    internal static async Task<ArrivalsResponse> HandleCtaApiResponse(this HttpResponseMessage response)
    {
        // Ensure we have a successul response code, or throw a service exception
        if (!response.IsSuccessStatusCode)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(response.StatusCode);
                case HttpStatusCode.RequestTimeout:
                    throw new ServiceTimedOutException(response.StatusCode);
                case HttpStatusCode.NotImplemented:
                case HttpStatusCode.BadGateway:
                case HttpStatusCode.ServiceUnavailable:
                case HttpStatusCode.GatewayTimeout:
                    throw new ServiceUnavailableException(response.StatusCode);
                default:
                    throw new ServiceException(response.StatusCode);
            }
        }

        // Attempt to deserialize the response
        var responseContent = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<CtaApiResult>(responseContent, JsonOptions) ??
            throw new JsonException($"Unable to deserialize payload: [{responseContent}]");

        // Extract any errors from the response. If there are none, return the deserialized object
        switch (result.Response.ErrorCode)
        {
            case ErrorCode.None:
                return result.Response;
            case ErrorCode.MissingParameter:
                throw new MissingParameterException(result.Response.ErrorDescription);
            case ErrorCode.InvalidApiKey:
                throw new InvalidApiKeyException(result.Response.ErrorDescription);
            case ErrorCode.DailyLimitExceeded:
                throw new DailyLimitExceededException(result.Response.ErrorDescription);
            case ErrorCode.InvalidMapId:
            case ErrorCode.MapIdNotInteger:
            case ErrorCode.InvalidStopId:
            case ErrorCode.StopIdNotInteger:
            case ErrorCode.InvalidMaxParam:
            case ErrorCode.NonPositiveMaxParam:
            case ErrorCode.InvalidRoute:
            case ErrorCode.InvalidParameter:
                throw new InvalidParameterException(result.Response.ErrorCode, result.Response.ErrorDescription);
            case ErrorCode.MaxMapIdsExceeded:
            case ErrorCode.MaxStopIdsExceeded:
            case ErrorCode.MaxRoutesExceeded:
                throw new MaxValuesExceededException(result.Response.ErrorCode, result.Response.ErrorDescription);
            case ErrorCode.ServerError:
                throw new ServerErrorException(result.Response.ErrorDescription);
            default:
                throw new CtaException(result.Response.ErrorCode, result.Response.ErrorDescription);
        }
    }
}
