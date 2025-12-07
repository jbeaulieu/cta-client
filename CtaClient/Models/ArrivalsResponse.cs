using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Represents a response from the CTA Arrivals API
/// </summary>
public class ArrivalsResponse
{
    /// <summary>
    ///   Timestamp of when the response was generated. Given in local Chicago time.
    /// </summary>
    [JsonPropertyName("tmst")]
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    ///   Numeric error code
    /// </summary>
    [JsonPropertyName("errCd")]
    public int ErrorCode { get; set; }

    /// <summary>
    ///   Textual error description/message
    /// </summary>
    [JsonPropertyName("errNm")]
    public string? ErrorName { get; set; }

    /// <summary>
    ///   List of arrival predictions
    /// </summary>
    [JsonPropertyName("eta")]
    public List<Arrival> Arrivals { get; set; } = [];
}
