using CtaClient.Config;
using CtaClient.Models;
using Microsoft.AspNetCore.WebUtilities;
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

        // Required query params
        var baseQueryParams = new Dictionary<string, string?>{{"key", ApiKey}, {"outputType", "JSON"}};

        var endpoint = QueryHelpers.AddQueryString(BaseAddress, baseQueryParams);

        // Optional request params
        var requestParams = new Dictionary<string, string?>();

        if (request.MapIds != null) requestParams.Add("mapid", request.MapIds.ToString());
        if (request.StopIds != null) requestParams.Add("stpid", request.StopIds.ToString());
        if (request.Routes != null) requestParams.Add("rt", ((Route)request.Routes).GetServiceId());
        if (request.MaxResults != null) requestParams.Add("max", request.MaxResults.ToString());

        endpoint = QueryHelpers.AddQueryString(endpoint, requestParams);

        return new Uri(endpoint, UriKind.Absolute);
    }
}
