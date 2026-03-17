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
        try
        {
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<CtaApiResult<T>>(responseContent, JsonOptions) ??
                throw new JsonException($"Unable to deserialize payload: [{responseContent}]");

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
        catch (Exception ex)
        {
            throw new CtaClientException("[HandleCtaApiResponse] An error occurred while processing CTA response", ex);
        }
    }
}
