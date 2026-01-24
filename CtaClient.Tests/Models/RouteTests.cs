using CtaClient.Models;

namespace CtaClient.Tests.Models;

public class RouteTests
{
    private readonly int invalidRouteEnum = -111;

    [Theory]
    [InlineData(Route.Red, "Red")]
    [InlineData(Route.Blue, "Blue")]
    [InlineData(Route.Brown, "Brn")]
    [InlineData(Route.Green, "G")]
    [InlineData(Route.Orange, "Org")]
    [InlineData(Route.Pink, "Pink")]
    [InlineData(Route.Purple, "P")]
    [InlineData(Route.Yellow, "Y")]
    public void RouteExtensions_ToServiceId(Route route, string serviceId)
    {
        Assert.Equal(serviceId, route.ToServiceId());
    }

    [Fact]
    public void RouteExtensions_ToServiceId_Throws()
    {
        Route route = (Route) invalidRouteEnum;

        Assert.Throws<ArgumentException>(() => route.ToServiceId());
    }
}
