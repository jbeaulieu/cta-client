using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Models;

namespace CtaClient.Json;

/// <summary>
///   Custom JSON converter for handling Route values returned by the CTA API.
/// </summary>
/// <remarks>
///   Write operations use the default behavior. Read operations use the <see cref="TryParseRoute" /> helper method
///     for strings, to handle the CTA's unique way of conveying Route values.
/// </remarks>
internal class JsonRouteConverter : JsonConverter<Route>
{
    public override Route Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
    reader.TokenType switch
    {
        JsonTokenType.String => TryParseRoute(reader),
        _ => throw new JsonException("[JsonRouteConverter] Encountered a non-string value"),
    };

    public override void Write(Utf8JsonWriter writer, Route value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.GetServiceId());
    }

    /// <summary>
    ///   Helper for reading stringified <see cref="Route" /> values.
    /// </summary>
    /// <remarks>
    ///   Unfortunately, the CTA API has multiple ways of returning routes in its response JSON. Within Train Tracker API
    ///   responses, routes are serialized using their service id, i.e. "Brn" for brown line trains. In Follow This Train API
    ///   responses, routes are serialized using human-readable values, i.e. "Brown Line" for brown line trains.
    ///   This handler will parse both sets of values into a standard <see cref="Route" /> enum.
    /// </remarks>
    private static Route TryParseRoute(Utf8JsonReader reader)
    {
        var input = reader.GetString() ?? throw new JsonException("[JsonRouteConverter] Encountered a null value");

        // We truncate strings like "Purple Line" to "Purple" before parsing
        if (input.EndsWith(" Line")){
            input = input.Split(' ')[0].Trim();
        }

        return input switch
        {
            _ when input == Route.Red.GetServiceId() || input == Route.Red.GetName()                     => Route.Red,
            _ when input == Route.Blue.GetServiceId() || input == Route.Blue.GetName()                   => Route.Blue,
            _ when input == Route.Brown.GetServiceId() || input == Route.Brown.GetName()                 => Route.Brown,
            _ when input == Route.Green.GetServiceId() || input == Route.Green.GetName()                 => Route.Green,
            _ when input == Route.Orange.GetServiceId() || input == Route.Orange.GetName()               => Route.Orange,
            _ when input == Route.Pink.GetServiceId() || input == Route.Pink.GetName()                   => Route.Pink,
            _ when input == Route.Purple.GetServiceId() || input == Route.Purple.GetName()                  => Route.Purple,
            _ when input == Route.PurpleExpress.GetServiceId() || input == Route.PurpleExpress.GetName() => Route.PurpleExpress,
            _ when input == Route.Yellow.GetServiceId() || input == Route.Yellow.GetName()               => Route.Yellow,
            _ => throw new JsonException($"[JsonRouteConverter] Could not deserialize JSON value {reader.GetString()} to Route"),
        };
    }
}
