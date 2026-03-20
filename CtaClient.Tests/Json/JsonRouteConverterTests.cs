using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Json;
using CtaClient.Models;

namespace CtaClient.Tests.Json;

public class JsonRouteConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonRouteConverter(),
        },
    };

    // Helper class to test deserializing json values to Routes using JsonRouteConverter
    private class RouteTestClass {
        public Route RouteCode { get; set; }
        public Route RouteAsString { get; set; }
        public Route RouteAsStringWithSuffix { get; set; }
    };

    [Theory]
    [InlineData(Route.Red, "Red", "Red", "Red Line")]
    [InlineData(Route.Blue, "Blue", "Blue", "Blue Line")]
    [InlineData(Route.Brown, "Brn", "Brown", "Brown Line")]
    [InlineData(Route.Green, "G", "Green", "Green Line")]
    [InlineData(Route.Orange, "Org", "Orange", "Orange Line")]
    [InlineData(Route.Pink, "Pink", "Pink", "Pink Line")]
    [InlineData(Route.Purple, "P", "Purple", "Purple Line")]
    [InlineData(Route.PurpleExpress, "Pexp", "PurpleExpress", "PurpleExpress Line")]
    [InlineData(Route.Yellow, "Y", "Yellow", "Yellow Line")]
    public void RouteConverterTests(Route route, string routeCode, string routeString, string routeStringWithSuffix)
    {
        string json = "{" +
            $"\"RouteCode\":\"{routeCode}\"," +
            $"\"RouteAsString\":\"{routeString}\"," +
            $"\"RouteAsStringWithSuffix\":\"{routeStringWithSuffix}\"" +
            "}";

        var result = JsonSerializer.Deserialize<RouteTestClass>(json, JsonOptions);

        Assert.NotNull(result);

        Assert.Equal(route, result.RouteCode);
        Assert.Equal(route, result.RouteAsString);
        Assert.Equal(route, result.RouteAsStringWithSuffix);
    }
}
