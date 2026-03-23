using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using CtaClient.Json;
using CtaClient.Models;

namespace CtaClient.Tests.Json;

public class JsonUtilsTests
{
    /// <summary>
    ///   Helper class for wrapping a valid CTA response.
    ///   Used for test cases where we don't want to ignore the root element.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private class CtaApiResult<T> where T : AbstractCtaResponse
    {
        [JsonPropertyName("ctatt")]
        public required T Response { get; set; }
    }

    [Fact]
    public void Deserialize_CtaApiResult_ArrivalsResponse_Runs()
    {
        string rootResponse = "{\"ctatt\":{\"tmst\":\"2025-12-05T17:54:32\",\"errCd\":\"0\",\"errNm\":null,\"eta\":[{\"staId\":\"41320\",\"stpId\":\"30258\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Loop\",\"rn\":\"523\",\"rt\":\"P\",\"destSt\":\"30203\",\"destNm\":\"Loop\",\"trDr\":\"5\",\"prdt\":\"2025-12-05T17:54:20\",\"arrT\":\"2025-12-05T17:55:20\",\"isApp\":\"1\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.94775\",\"lon\":\"-87.65363\",\"heading\":\"179\"},{\"staId\":\"41320\",\"stpId\":\"30255\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Howard or Linden\",\"rn\":\"924\",\"rt\":\"Red\",\"destSt\":\"30173\",\"destNm\":\"Howard\",\"trDr\":\"1\",\"prdt\":\"2025-12-05T17:53:44\",\"arrT\":\"2025-12-05T17:55:44\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.93161\",\"lon\":\"-87.65307\",\"heading\":\"357\"}]}}";

        var result = JsonUtils.Deserialize<CtaApiResult<ArrivalsResponse>>(rootResponse, ignoreRoot: false);

        Assert.IsType<CtaApiResult<ArrivalsResponse>>(result);
        Assert.IsType<ArrivalsResponse>(result.Response);
    }

    [Fact]
    public void Deserialize_CtaApiResult_ArrivalsResponse_IgnoreRoot_Runs()
    {
        string rootResponse = "{\"ctatt\":{\"tmst\":\"2025-12-05T17:54:32\",\"errCd\":\"0\",\"errNm\":null,\"eta\":[{\"staId\":\"41320\",\"stpId\":\"30258\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Loop\",\"rn\":\"523\",\"rt\":\"P\",\"destSt\":\"30203\",\"destNm\":\"Loop\",\"trDr\":\"5\",\"prdt\":\"2025-12-05T17:54:20\",\"arrT\":\"2025-12-05T17:55:20\",\"isApp\":\"1\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.94775\",\"lon\":\"-87.65363\",\"heading\":\"179\"},{\"staId\":\"41320\",\"stpId\":\"30255\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Howard or Linden\",\"rn\":\"924\",\"rt\":\"Red\",\"destSt\":\"30173\",\"destNm\":\"Howard\",\"trDr\":\"1\",\"prdt\":\"2025-12-05T17:53:44\",\"arrT\":\"2025-12-05T17:55:44\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.93161\",\"lon\":\"-87.65307\",\"heading\":\"357\"}]}}";

        var result = JsonUtils.Deserialize<ArrivalsResponse>(rootResponse, ignoreRoot: true);

        Assert.IsType<ArrivalsResponse>(result);

        Assert.IsType<DateTimeOffset>(result.Timestamp);
        Assert.Equal(DateTimeOffset.Parse("2025-12-05T17:54:32", new CultureInfo("en-US")), result.Timestamp);

        Assert.IsType<ErrorCode>(result.ErrorCode);
        Assert.Equal(ErrorCode.None, result.ErrorCode);

        Assert.Null(result.ErrorDescription);

        Assert.IsType<List<Arrival>>(result.Arrivals);
        Assert.Equal(2, result.Arrivals.Count);
    }

    [Fact]
    public void Deserialize_ArrivalsResponse_Runs()
    {
        string rootResponse = "{\"tmst\":\"2025-12-05T17:54:32\",\"errCd\":\"100\",\"errNm\":\"Required parameter 'key' is missing.\",\"eta\":[{\"staId\":\"41320\",\"stpId\":\"30258\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Loop\",\"rn\":\"523\",\"rt\":\"P\",\"destSt\":\"30203\",\"destNm\":\"Loop\",\"trDr\":\"5\",\"prdt\":\"2025-12-05T17:54:20\",\"arrT\":\"2025-12-05T17:55:20\",\"isApp\":\"1\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.94775\",\"lon\":\"-87.65363\",\"heading\":\"179\"},{\"staId\":\"41320\",\"stpId\":\"30255\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Howard or Linden\",\"rn\":\"924\",\"rt\":\"Red\",\"destSt\":\"30173\",\"destNm\":\"Howard\",\"trDr\":\"1\",\"prdt\":\"2025-12-05T17:53:44\",\"arrT\":\"2025-12-05T17:55:44\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.93161\",\"lon\":\"-87.65307\",\"heading\":\"357\"}]}";

        var result = JsonUtils.Deserialize<ArrivalsResponse>(rootResponse);

        Assert.IsType<ArrivalsResponse>(result);

        Assert.IsType<DateTimeOffset>(result.Timestamp);
        Assert.Equal(DateTimeOffset.Parse("2025-12-05T17:54:32", new CultureInfo("en-US")), result.Timestamp);

        Assert.IsType<ErrorCode>(result.ErrorCode);
        Assert.Equal(ErrorCode.MissingParameter, result.ErrorCode);

        Assert.IsType<string>(result.ErrorDescription);
        Assert.Equal("Required parameter 'key' is missing.", result.ErrorDescription);

        Assert.IsType<List<Arrival>>(result.Arrivals);
        Assert.Equal(2, result.Arrivals.Count);
    }

    [Fact]
    public void Deserialize_Arrival_Runs()
    {
        string rootResponse = "{\"staId\":\"41320\",\"stpId\":\"30258\",\"staNm\":\"Belmont\",\"stpDe\":\"Service toward Loop\",\"rn\":\"523\",\"rt\":\"P\",\"destSt\":\"30203\",\"destNm\":\"Loop\",\"trDr\":\"5\",\"prdt\":\"2025-12-05T17:54:20\",\"arrT\":\"2025-12-05T17:55:20\",\"isApp\":\"1\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null,\"lat\":\"41.94775\",\"lon\":\"-87.65363\",\"heading\":\"179\"}";

        var result = JsonUtils.Deserialize<Arrival>(rootResponse);

        Assert.IsType<Arrival>(result);

        Assert.IsType<int>(result.StationId);
        Assert.Equal(41320, result.StationId);

        Assert.IsType<int>(result.StopId);
        Assert.Equal(30258, result.StopId);

        Assert.IsType<string>(result.StationName);
        Assert.Equal("Belmont", result.StationName);

        Assert.IsType<string>(result.StationName);
        Assert.Equal("Service toward Loop", result.StopDescription);

        Assert.IsType<int>(result.RunNumber);
        Assert.Equal(523, result.RunNumber);

        Assert.IsType<Route>(result.Route);
        Assert.Equal(Route.Purple, result.Route);

        Assert.IsType<int>(result.DestinationStopId);
        Assert.Equal(30203, result.DestinationStopId);

        Assert.IsType<string>(result.DestinationName);
        Assert.Equal("Loop", result.DestinationName);

        Assert.IsType<int>(result.DirectionCode);
        Assert.Equal(5, result.DirectionCode);

        Assert.IsType<DateTimeOffset>(result.Timestamp);
        Assert.Equal(DateTimeOffset.Parse("2025-12-05T17:54:20", new CultureInfo("en-US")), result.Timestamp);

        Assert.IsType<DateTimeOffset>(result.ArrivalTime);
        Assert.Equal(DateTimeOffset.Parse("2025-12-05T17:55:20", new CultureInfo("en-US")), result.ArrivalTime);

        Assert.IsType<bool>(result.IsApproaching);
        Assert.True(result.IsApproaching);

        Assert.IsType<bool>(result.IsScheduled);
        Assert.False(result.IsScheduled);

        Assert.IsType<bool>(result.IsDelayed);
        Assert.False(result.IsDelayed);

        Assert.IsType<bool>(result.IsFaulty);
        Assert.False(result.IsFaulty);

        Assert.Null(result.Flags);

        // Exact floating-point equality comparisons are tricky because of precision issues.
        // The CTA only provides 5 decimal places of lat/lon data, so we use Math.Round here to
        // only check those digits for equality
        Assert.IsType<float>(result.Latitude);
        Assert.Equal(Math.Round(41.94775, 5), Math.Round((double)result.Latitude, 5));

        Assert.IsType<float>(result.Longitude);
        Assert.Equal(Math.Round(-87.65363, 5), Math.Round((double)result.Longitude, 5));

        Assert.IsType<int>(result.Heading);
        Assert.Equal(179, result.Heading);
    }

    [Fact]
    public void Deserialize_CtaApiResult_FollowTrainResponse_Runs()
    {
        string rootResponse = "{\"ctatt\":{\"tmst\":\"2026-02-27T09:50:34\",\"errCd\":\"100\",\"errNm\":\"Required parameter 'key' is missing.\",\"position\":{\"lat\":\"41.93739\",\"lon\":\"-87.65332\",\"heading\":\"177\"},\"eta\":[{\"staId\":\"41220\",\"stpId\":\"30234\",\"staNm\":\"Fullerton\",\"stpDe\":\"Service toward 95th/Dan Ryan\",\"rn\":\"810\",\"rt\":\"Red Line\",\"destSt\":\"30089\",\"destNm\":\"95th/Dan Ryan\",\"trDr\":\"5\",\"prdt\":\"2026-02-27T09:49:10\",\"arrT\":\"2026-02-27T09:51:10\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null},{\"staId\":\"40650\",\"stpId\":\"30126\",\"staNm\":\"North/Clybourn\",\"stpDe\":\"Service toward 95th/Dan Ryan\",\"rn\":\"810\",\"rt\":\"Red Line\",\"destSt\":\"30089\",\"destNm\":\"95th/Dan Ryan\",\"trDr\":\"5\",\"prdt\":\"2026-02-27T09:49:10\",\"arrT\":\"2026-02-27T09:54:10\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null}]}}";

        var result = JsonUtils.Deserialize<CtaApiResult<FollowTrainResponse>>(rootResponse);

        Assert.IsType<CtaApiResult<FollowTrainResponse>>(result);
        Assert.IsType<FollowTrainResponse>(result.Response);
    }

    [Fact]
    public void Deserialize_FollowTrainResponse_Runs()
    {
        string rootResponse = "{\"tmst\":\"2026-02-27T09:50:34\",\"errCd\":\"100\",\"errNm\":\"Required parameter 'key' is missing.\",\"position\":{\"lat\":\"41.93739\",\"lon\":\"-87.65332\",\"heading\":\"177\"},\"eta\":[{\"staId\":\"41220\",\"stpId\":\"30234\",\"staNm\":\"Fullerton\",\"stpDe\":\"Service toward 95th/Dan Ryan\",\"rn\":\"810\",\"rt\":\"Red Line\",\"destSt\":\"30089\",\"destNm\":\"95th/Dan Ryan\",\"trDr\":\"5\",\"prdt\":\"2026-02-27T09:49:10\",\"arrT\":\"2026-02-27T09:51:10\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null},{\"staId\":\"40650\",\"stpId\":\"30126\",\"staNm\":\"North/Clybourn\",\"stpDe\":\"Service toward 95th/Dan Ryan\",\"rn\":\"810\",\"rt\":\"Red Line\",\"destSt\":\"30089\",\"destNm\":\"95th/Dan Ryan\",\"trDr\":\"5\",\"prdt\":\"2026-02-27T09:49:10\",\"arrT\":\"2026-02-27T09:54:10\",\"isApp\":\"0\",\"isSch\":\"0\",\"isDly\":\"0\",\"isFlt\":\"0\",\"flags\":null}]}";

        var result = JsonUtils.Deserialize<FollowTrainResponse>(rootResponse);

        Assert.IsType<FollowTrainResponse>(result);

        Assert.IsType<DateTimeOffset>(result.Timestamp);
        Assert.Equal(DateTimeOffset.Parse("2026-02-27T09:50:34", new CultureInfo("en-US")), result.Timestamp);

        Assert.IsType<ErrorCode>(result.ErrorCode);
        Assert.Equal(ErrorCode.MissingParameter, result.ErrorCode);

        Assert.IsType<string>(result.ErrorDescription);
        Assert.Equal("Required parameter 'key' is missing.", result.ErrorDescription);

        Assert.IsType<TrainPosition>(result.Position);

        Assert.IsType<List<Arrival>>(result.Arrivals);
        Assert.Equal(2, result.Arrivals.Count);
    }

    [Fact]
    public void Deserialize_TrainPosition_Runs()
    {
        string rootResponse = "{\"lat\":\"41.93739\",\"lon\":\"-87.65332\",\"heading\":\"177\"}";

        var result = JsonUtils.Deserialize<TrainPosition>(rootResponse);

        Assert.IsType<TrainPosition>(result);

        // Exact floating-point equality comparisons are tricky because of precision issues.
        // The CTA only provides 5 decimal places of lat/lon data, so we use Math.Round here to
        // only check those digits for equality
        Assert.IsType<float>(result.Latitude);
        Assert.Equal(Math.Round(41.93739, 5), Math.Round((double)result.Latitude, 5));

        Assert.IsType<float>(result.Longitude);
        Assert.Equal(Math.Round(-87.65332, 5), Math.Round((double)result.Longitude, 5));

        Assert.IsType<int>(result.Heading);
        Assert.Equal(177, result.Heading);
    }
}
