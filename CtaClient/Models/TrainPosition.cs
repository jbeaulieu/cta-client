using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a snapshot of the position of a CTA train, at the time a request was made
/// </summary>
public class TrainPosition
{
    /// <summary>
    ///   The current latitude position of this train, in decimal degrees
    /// </summary>
    [JsonPropertyName("lat")]
    public float Latitude { get; set; }

    /// <summary>
    ///   The current longitude position of this train, in decimal degrees
    /// </summary>
    [JsonPropertyName("lon")]
    public float Longitude { get; set; }

    /// <summary>
    ///   The current heading of this train, expressed in standard bearing degrees
    ///     (0 = North, 90 = East, 180 = South, and 270 = West. Range is 0 to 359, progressing clockwise)
    /// </summary>
    [JsonPropertyName("heading")]
    public int Heading { get; set; }
}
