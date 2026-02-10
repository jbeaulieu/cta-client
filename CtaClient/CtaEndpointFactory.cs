using CtaClient.Config;
using CtaClient.Exceptions;
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
        // Validation: At least one of request.MapId and request.StopId should be populated.
        if (IsNullOrEmpty(request.MapIds) && IsNullOrEmpty(request.StopIds))
        {
            throw new MissingParameterException("MapId or StopId must be specified");
        }

        // Required query params
        var baseQueryParams = new Dictionary<string, string?>{{"key", ApiKey}, {"outputType", "JSON"}};

        var endpoint = QueryHelpers.AddQueryString(BaseAddress, baseQueryParams);

        // Optional request params
        var requestParams = new List<KeyValuePair<string, string?>>();

        foreach (var mapId in request.MapIds ?? [])
        {
            requestParams.Add(new KeyValuePair<string, string?>("mapid", mapId.ToString()));
        }

        foreach (var stopId in request.StopIds ?? [])
        {
            requestParams.Add(new KeyValuePair<string, string?>("stpid", stopId.ToString()));
        }

        foreach (var route in request.Routes ?? [])
        {
            requestParams.Add(new KeyValuePair<string, string?>("rt", route.GetServiceId()));
        }

        if (request.MaxResults != null)
        {
            requestParams.Add(new KeyValuePair<string, string?>("max", request.MaxResults.ToString()));
        }

        endpoint = QueryHelpers.AddQueryString(endpoint, requestParams);

        return new Uri(endpoint, UriKind.Absolute);
    }

    /// <summary>
    ///   Validates that a list is not null and has at least one element.
    /// </summary>
    private static bool IsNullOrEmpty(List<int>? list) => list == null || list.Count == 0;
}
