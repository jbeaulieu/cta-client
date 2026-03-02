using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a response from the CTA "Follow This Train" API
/// </summary>
public class FollowTrainResponse: AbstractCtaResponse
{
    /// <summary>
    ///   The current position of the train
    /// </summary>
    [JsonPropertyName("position")]
    public TrainPosition Position { get; set; } = new();

    /// <summary>
    ///   List of arrival predictions
    /// </summary>
    [JsonPropertyName("eta")]
    public List<Arrival> Arrivals { get; set; } = [];
}
