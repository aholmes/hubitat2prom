using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using hubitat2prom.PrometheusExporter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hubitat2prom.Controllers;

[ApiController]
[Route("[controller]")]
public class HubitatController : ControllerBase
{
    private readonly ILogger<HubitatController> _logger;
    private readonly HubitatEnv _env;
    private readonly Hubitat _hubitat;

    public HubitatController(Hubitat hubitat, HubitatEnv env, ILogger<HubitatController> logger)
    {
        _hubitat = hubitat;
        _env = env;
        _logger = logger;
    }

    [HttpGet]
    [Route("/info")]
    public async Task<ExporterInfo> GetInfo()
    {
        var statusCode = await _hubitat.StatusCheck();

        return new ExporterInfo
        {
            status = new ExporterInfoStatus
            {
                CONNECTION = statusCode == HttpStatusCode.OK
                    ? "ONLINE"
                    : "OFFLINE"
            },
            config = HubitatEnv.Instance
            with
            {
                HE_TOKEN = Guid.Empty
            }
        };
    }

    [HttpGet]
    [Route("/metrics")]
    public async Task<ContentResult> GetMetrics()
    {
        var devices = await _hubitat.Devices();

        var deviceDetails = await _hubitat.DeviceDetails();

        var responseContent = new StringBuilder();
        var deviceAttributes = HubitatDeviceMetrics.Export(deviceDetails, HubitatEnv.Instance.HE_METRICS);
        foreach (var deviceAttribute in deviceAttributes)
        {
            responseContent.AppendLine(deviceAttribute.ToString());
        }

        return new ContentResult
        {
            Content = responseContent.ToString()
        };
    }

    [HttpGet]
    [Route("/metrics/{deviceId:int}")]
    public async Task<ContentResult> GetMetrics(int deviceId)
    {
        var devices = await _hubitat.Devices();

        var deviceDetails = await _hubitat.DeviceDetails(deviceId);

        var responseContent = new StringBuilder();
        var deviceAttributes = HubitatDeviceMetrics.Export(new[] { deviceDetails }, HubitatEnv.Instance.HE_METRICS);
        foreach (var deviceAttribute in deviceAttributes)
        {
            responseContent.AppendLine(deviceAttribute.ToString());
        }

        return new ContentResult
        {
            Content = responseContent.ToString()
        };
    }
}