using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using Serilog;
using CtaClient.Extensions;
using CtaClient.Models;

namespace CtaClient.Util.Console;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        var services = CreateServices(configuration);
        ICtaClient client = services.GetRequiredService<ICtaClient>();

        var jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        var request = new ArrivalsRequest
        {
            MapIds = [41320],
            MaxResults = 2
        };

        var result = await client.GetArrivals(request);

        System.Console.WriteLine(JsonSerializer.Serialize(result, jsonOptions));
    }

    private static ServiceProvider CreateServices(IConfiguration configuration)
    {
        var serviceProvider = new ServiceCollection()
            .AddCtaClient(configuration)
            .AddSerilog(new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug()
                .MinimumLevel.Debug()
                .CreateLogger())
            .BuildServiceProvider();

        return serviceProvider;
    }
}
