using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace CtaClient.Json;

internal static class JsonUtils
{
    private static readonly JsonSerializerOptions DefaultJsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = false,
        Converters =
        {
            new JsonBoolConverter(),
            new JsonRouteConverter(),
            new JsonStringEnumConverter(),
        },
    };

    /// <summary>
    ///   Attempts to deserialize a json string to an object of type T, optionally ignoring the root element of the json string.
    /// </summary>
    /// <remarks>
    ///   The CTA APIs encapsulate their responses in different root element types; e.g. Train Tracker API responses
    ///   are returned inside of a "ctatt" object, while Alerts API responses are returned inside of a "CTAAlerts" object.
    ///   Ignoring the root element allows us to avoid needing separate classes for each of these named containers.
    /// </remarks>
    internal static T Deserialize<T>(string json, JsonSerializerOptions? jsonOptions = null, bool ignoreRoot = false) where T : class
    {
        T? result = null;

        JsonSerializerOptions options = jsonOptions ?? DefaultJsonOptions;

        if (ignoreRoot)
        {
            var jsonElement = JsonNode.Parse(json)?[0]?.AsObject() ??
                throw new JsonException($"[JsonUtils] Encountered null object when trying to deserialize {nameof(T)}");

            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            jsonElement.WriteTo(writer);
            writer.Flush();

            result = JsonSerializer.Deserialize<T>(stream.ToArray(), options);
        }
        else
        {
            result = JsonSerializer.Deserialize<T>(json, options);
        }

        return result ?? throw new JsonException($"[JsonUtils] Deserializing type {nameof(T)} produced null result");
    }
}
