using System.Reflection;
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
    [JsonStringEnumMemberName("Pexp")]
    PurpleExpress,
    [JsonStringEnumMemberName("Y")]
    Yellow
}

public static class RouteExtensions
{
    /// <summary>
    ///   Get the human-readable name for this route.
    /// </summary>
    public static string GetName(this Route route)
    {
        return route.ToString();
    }

    /// <summary>
    ///   Static utility that uses Reflection to return the route code as used by the CTA. This method is called when
    ///     the Client calls a CTA API endpoint with a route query parameter, and needs to serialize the route name to the
    ///     value expected by the CTA.
    /// </summary>
    /// <param name="route"></param>
    /// <returns>String of the route code as used by the CTA API</returns>
    /// <exception cref="ArgumentException"></exception>
    public static string GetServiceId(this Route route)
    {
        string? name = Enum.GetName(typeof(Route), route);
        if (name != null)
        {
            FieldInfo? field = typeof(Route).GetField(name);
            if (field != null &&
                Attribute.GetCustomAttribute(field, typeof(JsonStringEnumMemberNameAttribute)) is JsonStringEnumMemberNameAttribute attr)
            {
                return attr.Name;
            }
        }

        throw new ArgumentException($"Invalid enum for Route: {route}");
    }
}
