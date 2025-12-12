using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Interfaces;
using CtaClient.Models;
using CtaClient.Util;
using Microsoft.Extensions.Logging;

namespace CtaClient;

public class CtaClient(HttpClient httpClient, CtaEndpointFactory endpointFactory, ILogger<CtaClient> logger): ICtaClient
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

    /// <inheritdoc />
    public async Task<ArrivalsResponse> GetArrivals(ArrivalsRequest request)
    {
        logger.LogInformation("[CtaClient] GetArrivals() called.");

        var uri = endpointFactory.GetArrvialsEndpoint(request);

        var response = await SendAsync(uri);

        var result = await response.Content.ReadFromJsonAsync<CtaApiResult>(JsonOptions) ??
            // TODO: Create custom exception for invalid response
            throw new InvalidOperationException($"Unable to deserialize payload: [{response.Content.ReadAsStringAsync()}]");

        return result.Response;
    }

    private async Task<HttpResponseMessage> SendAsync(Uri requestUri)
    {
        if (logger.IsEnabled(LogLevel.Information)) logger.LogInformation("[CtaClient] Invoking Endpoint: GET {Uri}", requestUri);

        var response = await httpClient.GetAsync(requestUri);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                logger.LogDebug("[CtaClient] Response Content: {Content}", await response.Content.ReadAsStringAsync());
            }
            catch(Exception ex)
            {
                logger.LogDebug(ex, "[CtaClient] Response Content could not be read as string");
            }
        }

        return response;
    }
}
