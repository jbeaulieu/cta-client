using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a request to the CTA Arrivals API
/// </summary>
public class ArrivalsRequest
{
    /// <summary>
    ///   Numeric station identifiers. Required if StopIds is not specified
    /// </summary>
    [JsonPropertyName("mapid")]
    public List<int>? MapIds { get; set; }

    /// <summary>
    ///   Numeric stop identifiers. Required if MapIds is not specified.
    /// </summary>
    [JsonPropertyName("stpid")]
    public List<int>? StopIds { get; set; }

    /// <summary>
    ///   Optional. Allows for filtering by individual routes. If unset, all routes for the requested stop
    ///     or station will be returned.
    /// </summary>
    [JsonPropertyName("rt")]
    public List<Route>? Routes { get; set; }

    /// <summary>
    ///   Optional. Used to specify the maximum number of results to retrieve, sorted by predicted arrival time.
    ///     If unset, all available results for the requested stop or station will be returned.
    /// </summary>
    [JsonPropertyName("max")]
    public int? MaxResults { get; set; }
}
