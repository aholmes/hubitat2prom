using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using hubitat2prom.PrometheusExporter;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hubitat2prom.Controllers;

/// <summary>
/// The controller for /info and /metrics
/// </summary>
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

    /// <summary>
    /// Get the current status of the Hubitat hub and data about the exporter.
    /// </summary>
    /// <returns>
    /// A valid response looks something like this:
    /// <example>
    /// {
    ///     "status": {
    ///         "CONNECTION": "ONLINE"
    ///     },
    ///     "config": {
    ///         "HE_URI": "http://192.168.50.22/apps/api/712/devices",
    ///          "HE_TOKEN": "00000000-0000-0000-0000-000000000000",
    ///          "HE_METRICS": [
    ///              "battery",
    ///              "humidity",
    ///              "level",
    ///              "switch",
    ///              ...
    ///          ]
    ///     }
    /// }
    /// </example>
    /// </returns>
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
            config = _env
            with
            {
                HE_TOKEN = Guid.Empty
            }
        };
    }

    /// <summary>
    /// Get the Prometheus exporter values for devices configured on the Hubitat hub.
    /// </summary>
    /// <returns>
    /// A valid response looks something like this:
    /// <example>
    /// switch{device_name="bathroom"} 0.0
    /// level{device_name="living_room"} 1.0
    /// </example>
    /// </returns>
    [HttpGet]
    [Route("/metrics")]
    public async Task<ContentResult> GetMetrics()
    {
        var deviceDetails = await _hubitat.DeviceDetails();

        var responseContent = new StringBuilder();
        var deviceAttributes = HubitatDeviceMetrics.Export(deviceDetails, _env.HE_METRICS);
        foreach (var deviceAttribute in deviceAttributes)
        {
            responseContent.AppendLine(deviceAttribute.ToString());
        }

        return new ContentResult
        {
            Content = responseContent.ToString()
        };
    }

    /// <summary>
    /// Get the Prometheus exporter values for a specific device configured on the Hubitat hub.
    /// </summary>
    /// <returns>
    /// A valid response looks something like this:
    /// <example>
    /// switch{device_name="bathroom"} 0.0
    /// </example>
    /// </returns>
    [HttpGet]
    [Route("/metrics/{deviceId:int}")]
    public async Task<ContentResult> GetMetrics(int deviceId)
    {
        var deviceDetails = await _hubitat.DeviceDetails(deviceId);

        var responseContent = new StringBuilder();
        var deviceAttributes = HubitatDeviceMetrics.Export(new[] { deviceDetails }, _env.HE_METRICS);
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