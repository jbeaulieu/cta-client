using System.Text.Json.Serialization;

namespace CtaClient.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Route
{
    [JsonStringEnumMemberName("Red")]
    Red,
    [JsonStringEnumMemberName("Blue")]
    Blue,
    [JsonStringEnumMemberName("Brn")]
    Brown,
    [JsonStringEnumMemberName("G")]
    Green,
    [JsonStringEnumMemberName("Org")]
    Orange,
    [JsonStringEnumMemberName("Pink")]
    Pink,
    [JsonStringEnumMemberName("P")]
    Purple,
    [JsonStringEnumMemberName("Y")]
    Yellow
}
