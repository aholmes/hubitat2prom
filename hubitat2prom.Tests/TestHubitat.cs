using Xunit;

using hubitat2prom;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace hubitat2prom.Tests;

public class TestHubitat
{
    private HttpClient _httpClient;
    public TestHubitat()
    {
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task Devices_Returns_Summary()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClient);
        var devices = await hubitat.Devices();
        Assert.NotEmpty(devices);
        foreach(var device in devices)
        {
            Assert.NotEmpty(device.id);
            Assert.NotEmpty(device.name);
            Assert.NotEmpty(device.label);
            Assert.NotEmpty(device.type);
        }
    }

    [Fact]
    public async Task DeviceDetails_Returns_Details()
    {
        var env = new HubitatEnv();
        var hubitat = new Hubitat(env.HE_URI, env.HE_TOKEN, _httpClient);
        var devices = await hubitat.Devices();

        var device = devices.First();

        var details = await hubitat.DeviceDetails(device);
        Assert.NotNull(details);
        Assert.NotEmpty(details.attributes);
        Assert.NotEmpty(details.capabilities);
    }
}