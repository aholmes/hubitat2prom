using Microsoft.Extensions.DependencyInjection;

namespace hubitat2prom.Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(typeof(MockCreator));
    }
}