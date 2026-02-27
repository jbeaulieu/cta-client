using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a response from the CTA Arrivals API
/// </summary>
public class ArrivalsResponse: AbstractCtaResponse
{
    /// <summary>
    ///   List of arrival predictions
    /// </summary>
    [JsonPropertyName("eta")]
    public List<Arrival> Arrivals { get; set; } = [];
}
