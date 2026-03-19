using CtaClient.Models;

namespace CtaClient;

public interface ICtaClient
{
    /// <summary>
    ///   Performs a GET request to the CTA Arrivals API, fetching train arrival data for a stop or station
    /// </summary>
    Task<Result<ArrivalsResponse>> GetArrivals(ArrivalsRequest request);

    /// <summary>
    ///   Performs a GET request to the CTA Follow This Train API. Produces a list of arrival predictions for
    ///     a given train, up to 60 minutes into the future or to the end of its trip.
    /// </summary>
    Task<Result<FollowTrainResponse>> FollowThisTrain(FollowTrainRequest request);
}
