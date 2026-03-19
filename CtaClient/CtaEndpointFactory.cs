using CtaClient.Config;
using CtaClient.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;

namespace CtaClient;

internal class CtaEndpointFactory
{
    private string BaseAddress { get; set; }
    private string ApiKey { get; set; }
    private const string CTA_ARRIVALS_PATH = "/ttarrivals.aspx";
    private const string CTA_FOLLOW_TRAIN_PATH = "/ttfollow.aspx";

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
    internal Uri GetArrivalsEndpoint(ArrivalsRequest request)
    {
        var builder = new UriBuilder(BaseAddress);
        builder.Path += CTA_ARRIVALS_PATH;

        var requestParams = new List<KeyValuePair<string, string?>>();

        // Required query params
        requestParams.AddRange([new("key", ApiKey), new("outputType", "JSON")]);

        // Optional request params
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

        var endpoint = QueryHelpers.AddQueryString(builder.ToString(), requestParams);

        return new Uri(endpoint, UriKind.Absolute);
    }

    /// <summary>
    ///   Validates and formats a uri request for the CTA Arrivals endpoint
    /// </summary>
    internal Uri GetFollowThisTrainEndpoint(FollowTrainRequest request)
    {
        var builder = new UriBuilder(BaseAddress);
        builder.Path += CTA_FOLLOW_TRAIN_PATH;

        var requestParams = new List<KeyValuePair<string, string?>>(){
            new("key", ApiKey),
            new("outputType", "JSON"),
            new("runnumber", request.RunNumber.ToString())
        };

        var endpoint = QueryHelpers.AddQueryString(builder.ToString(), requestParams);

        return new Uri(endpoint, UriKind.Absolute);
    }
}
