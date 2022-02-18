using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hubitat2prom.PrometheusModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace hubitat2prom.Controllers;

[ApiController]
[Route("[controller]")]
public class HubitatController : ControllerBase
{
    const string INVALID_CHARACTER_REGEX = "[() -]";

    private readonly ILogger<HubitatController> _logger;
    private readonly HubitatEnv _env;
    private readonly Hubitat _hubitat;
    private readonly string[] _collectedMetrics;

    public HubitatController(Hubitat hubitat, HubitatEnv env, ILogger<HubitatController> logger)
    {
        _hubitat = hubitat;
        _env = env;
        _logger = logger;

        _collectedMetrics = _env.HE_METRICS.Split(',');
    }

    [HttpGet]
    [Route("/info")]
    public async Task<PrometheusExporterInfo> GetInfo()
    {
        var statusCode = await _hubitat.StatusCheck();

        return new PrometheusExporterInfo
        {
            status = new PrometheusExporterStatus
            {
                CONNECTION = statusCode == HttpStatusCode.OK
                    ? "ONLINE"
                    : "OFFLINE"
            },
            config = new PrometheusExporterConfig
            {
                HE_URI = _env.HE_URI,
                HE_TOKEN = Guid.Empty,
                HE_METRICS = _collectedMetrics
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
        var deviceAttributes = HubitatPrometheus.GetPrometheusDeviceMetrics(deviceDetails, _collectedMetrics);
        foreach(var deviceAttribute in deviceAttributes)
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
        var deviceAttributes = HubitatPrometheus.GetPrometheusDeviceMetrics(new[] { deviceDetails }, _collectedMetrics);
        foreach(var deviceAttribute in deviceAttributes)
        {
            responseContent.AppendLine(deviceAttribute.ToString());
        }

        return new ContentResult
        {
            Content = responseContent.ToString()
        };
    }
}