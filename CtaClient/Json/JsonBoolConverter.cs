using System.Text.Json;
using System.Text.Json.Serialization;

namespace CtaClient.Json;

/// <summary>
///   Custom JSON converter for handling boolean values returned by the CTA API.
/// </summary>
/// <remarks>
///   Write operations use the default behavior. Read operations use the <see cref="TryParseString" /> helper method
///     for strings, to handle the CTA's unique way of conveying bool values.
/// </remarks>
public class JsonBoolConverter : JsonConverter<bool>
{
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
    reader.TokenType switch
    {
        JsonTokenType.True => true,
        JsonTokenType.False => false,
        JsonTokenType.String => TryParseString(reader),
        JsonTokenType.Number => reader.TryGetInt64(out long l) ? Convert.ToBoolean(l) : reader.TryGetDouble(out double d) && Convert.ToBoolean(d),
        _ => throw new JsonException(),
    };

    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue(value);
    }

    /// <summary>
    ///   Helper for reading stringified boolean values. The CTA API returns booleans as "0" and "1", i.e. an int cast as a string.
    ///     This handler will parse those values, as well as the more conventional "true" and "false" JSON strings.
    /// </summary>
    public static bool TryParseString(Utf8JsonReader reader)
    {
        if (bool.TryParse(reader.GetString(), out var b)) return b;

        if (int.TryParse(reader.GetString(), out var i)) return Convert.ToBoolean(i);
        
        throw new JsonException($"[JsonBoolConverter] Could not deserialize JSON value {reader.GetString()} to bool");
    }
}
