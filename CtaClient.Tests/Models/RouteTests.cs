using CtaClient.Models;

namespace CtaClient.Tests.Models;

public class RouteTests
{
    private readonly int invalidRouteEnum = -111;

    [Theory]
    [InlineData(Route.Red, "Red")]
    [InlineData(Route.Blue, "Blue")]
    [InlineData(Route.Brown, "Brown")]
    [InlineData(Route.Green, "Green")]
    [InlineData(Route.Orange, "Orange")]
    [InlineData(Route.Pink, "Pink")]
    [InlineData(Route.Purple, "Purple")]
    [InlineData(Route.PurpleExpress, "PurpleExpress")]
    [InlineData(Route.Yellow, "Yellow")]
    public void RouteExtensions_GetName_ValidEnum(Route route, string serviceId)
    {
        Assert.Equal(serviceId, route.GetName());
    }

    [Fact]
    public void RouteExtensions_GetName_InvalidEnum()
    {
        Route route = (Route) invalidRouteEnum;

        Assert.Equal(invalidRouteEnum.ToString(), route.GetName());
    }

    [Theory]
    [InlineData(Route.Red, "Red")]
    [InlineData(Route.Blue, "Blue")]
    [InlineData(Route.Brown, "Brn")]
    [InlineData(Route.Green, "G")]
    [InlineData(Route.Orange, "Org")]
    [InlineData(Route.Pink, "Pink")]
    [InlineData(Route.Purple, "P")]
    [InlineData(Route.PurpleExpress, "Pexp")]
    [InlineData(Route.Yellow, "Y")]
    public void RouteExtensions_ToServiceId_ValidEnum(Route route, string serviceId)
    {
        Assert.Equal(serviceId, route.GetServiceId());
    }

    [Fact]
    public void RouteExtensions_ToServiceId_InvalidEnum()
    {
        Route route = (Route) invalidRouteEnum;

        Assert.Throws<ArgumentException>(() => route.GetServiceId());
    }
}
