using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a request to the CTA "Follow This Train" API
/// </summary>
public class FollowTrainRequest
{
    /// <summary>
    ///   The run number of a train for which you’d like a series of upcoming arrival estimations.
    /// </summary>
    [JsonPropertyName("runnumber")]
    public int RunNumber { get; set; }
}
