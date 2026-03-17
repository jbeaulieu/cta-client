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
            new JsonRouteConverter(),
            new JsonStringEnumConverter(),
        },
    };

    /// <summary>
    ///   Validates that the response from the CTA API indicates success, and extracts the response
    ///     model if so. If the response indicates an error, either from the response status code or
    ///     a CTA error code, a corresponding exception is thrown.
    /// </summary>
    /// <typeparam name="T">
    ///     The type that the response should be deserialized to. Must inherit from <see cref="AbstractCtaResponse"/>.
    /// </typeparam>
    /// <param name="response">The Http response message to validate and deserialize.</param>
    /// <returns>The validated ArrivalsResponse deserialized from the message response</returns>
    internal static async Task<Result<T>> HandleCtaApiResponse<T>(this HttpResponseMessage response) where T : AbstractCtaResponse
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
        var result = JsonSerializer.Deserialize<CtaApiResult<T>>(responseContent, JsonOptions) ??
            throw new JsonException($"Unable to deserialize payload: [{responseContent}]");

        // If the error code of the response indicates success, return the deserialized result.
        // Otherwise, construct an Error and return it as a failed Result.
        if(result.Response.ErrorCode == ErrorCode.None)
        {
            return Result<T>.Success(result.Response);
        }
        else
        {
            Error error = new(result.Response.ErrorCode, result.Response.ErrorDescription ?? string.Empty);
            return Result<T>.Failure(error);
        }
    }
}
