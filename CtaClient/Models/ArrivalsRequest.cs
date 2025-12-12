using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a request to the CTA Arrivals API
/// </summary>
public class ArrivalsRequest
{
    /// <summary>
    ///   Numeric station identifier. Required if StopId is not specified
    /// </summary>
    [JsonPropertyName("mapid")]
    public int? MapId { get; set; }

    /// <summary>
    ///   Numeric stop identifier. Required if MapId is not specified
    /// </summary>
    [JsonPropertyName("stpid")]
    public int? StopId { get; set; }

    /// <summary>
    ///   Optional. Allows for filtering by individual routes. If unset, all routes for the requested stop
    ///     or station will be returned.
    /// </summary>
    [JsonPropertyName("rt")]
    public Route? Route { get; set; }

    /// <summary>
    ///   Optional. Allows for specifying the maximum number of results to retrieve. If unset, all available
    ///     results for the requested stop or station will be returned.
    /// </summary>
    [JsonPropertyName("max")]
    public int? MaxResults { get; set; }
}
