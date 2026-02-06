using Microsoft.Extensions.Logging;
using CtaClient.Extensions;
using CtaClient.Models;

namespace CtaClient;

internal class CtaClient(HttpClient httpClient, CtaEndpointFactory endpointFactory, ILogger<CtaClient> logger): ICtaClient
{
    /// <inheritdoc />
    public async Task<ArrivalsResponse> GetArrivals(ArrivalsRequest request)
    {
        logger.LogInformation("[CtaClient] GetArrivals() called.");

        var uri = endpointFactory.GetArrvialsEndpoint(request);

        var response = await SendAsync(uri);

        return await response.HandleCtaApiResponse();
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
