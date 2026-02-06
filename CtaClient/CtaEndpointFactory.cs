using CtaClient.Config;
using CtaClient.Models;
using Microsoft.Extensions.Options;

namespace CtaClient;

internal class CtaEndpointFactory
{
    private string BaseAddress { get; set; }
    private string ApiKey { get; set; }

    public CtaEndpointFactory(IOptions<CtaApiSettings> apiSettings)
    {
        if (string.IsNullOrWhiteSpace(apiSettings.Value.BaseAddress) ||
            string.IsNullOrWhiteSpace(apiSettings.Value.ApiKey))
        {
            throw new ArgumentNullException(nameof(apiSettings));
        }

        BaseAddress = apiSettings.Value.BaseAddress;
        ApiKey = apiSettings.Value.ApiKey;
    }

    /// <summary>
    ///   Validates and formats a uri request for the CTA Arrivals endpoint
    /// </summary>
    internal Uri GetArrvialsEndpoint(ArrivalsRequest request)
    {
        // Validation: Exactly one of request.MapId and request.StopId should be populated.
        if ((request.MapId != null && request.StopId != null) || (request.MapId == null && request.StopId == null))
        {
            throw new ArgumentException("Exactly one of MapId or StopId should be specified");
        }

        var endpoint = $"{BaseAddress}?key={ApiKey}&outputType=JSON";

        if (request.MapId != null) endpoint += $"&mapid={request.MapId}";
        if (request.StopId != null) endpoint += $"&stpid={request.StopId}";
        if (request.Route != null) endpoint += $"&rt={((Route)request.Route).GetServiceId()}";
        if (request.MaxResults != null) endpoint += $"&max={request.MaxResults}";

        return new Uri(endpoint, UriKind.Absolute);
    }
}
