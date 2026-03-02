using System.Text.Json.Serialization;

namespace CtaClient.Models;

public abstract class AbstractCtaResponse
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
    public ErrorCode ErrorCode { get; set; }

    /// <summary>
    ///   Textual error description/message
    /// </summary>
    [JsonPropertyName("errNm")]
    public string? ErrorDescription { get; set; }
}
