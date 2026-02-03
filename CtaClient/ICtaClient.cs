using CtaClient.Models;

namespace CtaClient;

public interface ICtaClient
{
    /// <summary>
    ///   Performs a GET request to the CTA Arrivals API, fetching train arrival data for a stop or station
    /// </summary>
    Task<ArrivalsResponse> GetArrivals(ArrivalsRequest request);
}
