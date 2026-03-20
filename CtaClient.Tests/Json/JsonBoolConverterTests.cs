using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Json;

namespace CtaClient.Tests.Json;

public class JsonBoolConverterTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        Converters =
        {
            new JsonBoolConverter(),
        },
    };

    // Helper class to test deserializing json values to booleans using JsonBoolConverter
    private class BooleanTestClass {
        public bool TrueLiteral { get; set; }
        public bool FalseLiteral { get; set; }
        public bool TrueString { get; set; }
        public bool FalseString { get; set; }
        public bool TrueInt { get; set; }
        public bool FalseInt { get; set; }
        public bool TrueIntAsString { get; set; }
        public bool FalseIntAsString { get; set; }
    };

    [Fact]
    public void BoolConverterTests()
    {
        string json = "{" +
            "\"TrueLiteral\":true," +
            "\"FalseLiteral\":false," +
            "\"TrueString\":\"true\"," +
            "\"FalseString\":\"false\"," +
            "\"TrueInt\":1," +
            "\"FalseInt\":0," +
            "\"TrueIntAsString\":\"1\"," +
            "\"FalseIntAsString\":\"0\"" +
            "}";

        var result = JsonSerializer.Deserialize<BooleanTestClass>(json, JsonOptions);

        Assert.NotNull(result);

        Assert.True(result.TrueLiteral);
        Assert.True(result.TrueString);
        Assert.True(result.TrueInt);
        Assert.True(result.TrueIntAsString);

        Assert.False(result.FalseLiteral);
        Assert.False(result.FalseString);
        Assert.False(result.FalseInt);
        Assert.False(result.FalseIntAsString);
    }
}
