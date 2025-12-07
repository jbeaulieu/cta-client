using System.Text.Json.Serialization;

namespace CtaClient.Models;

/// <summary>
///   Root node of any response from the CTA Train Tracker API.
///   All CTA Train Tracker responses are presented as JSON objects with a "ctatt" root element.
///   This class exists to help with deserialization as a representation of that root,
///   while making the actual response data more accessible in its own <see cref="ArrivalsResponse"/> class
/// </summary>
public class CtaApiResult
{
    /// <summary>
    ///   Numeric station identifier. Required if StopId is not specified
    /// </summary>
    [JsonPropertyName("ctatt")]
    public ArrivalsResponse? Response { get; set; }
}
