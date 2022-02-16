using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using hubitat2prom.HubitatModels;
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
                HE_METRICS = _env.HE_METRICS.Split(',')
            }
        };
    }

    [HttpGet]
    [Route("/metrics")]
    public async Task<List<PrometheusHubitatDeviceDetail>> GetMetrics()
    {
        var devices = await _hubitat.Devices();

        var deviceDetailTasks = devices.Select(_hubitat.DeviceDetails);
        var deviceDetails = await Task.WhenAll(deviceDetailTasks);

        var deviceAttributes = new List<PrometheusHubitatDeviceDetail>();

        foreach(var deviceDetail in deviceDetails)
        foreach(var attribute in deviceDetail.attributes)
        {
            if (!_collectedMetrics.Contains(attribute.name)) continue;
            if (!attribute.currentValue.HasValue) continue;

            var metricName = Regex.Replace(attribute.name, INVALID_CHARACTER_REGEX, "_");
            var deviceName = Regex.Replace(deviceDetail.label, INVALID_CHARACTER_REGEX, "_");
            
            var metricValue = attribute.currentValue.Value;

            switch(metricName)
            {
                case "switch":
                    metricValue = metricValue.AsT1 == "on"
                        ? 1
                        : 0;
                break;
                case "power":
                    if (TryGetPower(metricValue.AsT1, out int power))
                    {
                        metricValue = power;
                    }
                break;
                case "thermostatoperatingstate":
                    if (TryGetThermostatOperatingState(metricValue.AsT1, out int thermostatOperatingState))
                    {
                        metricValue = thermostatOperatingState;
                    }
                break;
                case "thermostatmode":
                    if (TryGetThermostatMode(metricValue.AsT1, out int thermostatMode))
                    {
                            metricValue = thermostatMode;
                    }
                break;
            }

            deviceAttributes.Add(new PrometheusHubitatDeviceDetail
            {
                DeviceName = deviceName,
                MetricName = metricName,
                MetricValue = metricValue,
                MetricTimestamp = DateTime.UnixEpoch
            });
        }

        return deviceAttributes;
    }
    
    private bool TryGetPower(string power, out int value)
    {
        if (power == "off")
        {
            value = 0;
            return true;
        }

        if (power == "on")
        {
            value = 1;
            return true;
        }

        value = -1;
        return false;
    }

    private bool TryGetThermostatOperatingState(string thermostatOperatingState, out int value)
    {
        switch(thermostatOperatingState)
        {
            case "heating": value = 0; return true;
            case "pending cool": value = 1; return true;
            case "pending heat": value = 2; return true;
            case "vent economizer": value = 3; return true;
            case "idle": value = 4; return true;
            case "cooling": value = 5; return true;
            case "fan only": value = 6; return true;
        }

        value = -1;
        return false;
    }
    
    private bool TryGetThermostatMode(string thermostatMode, out int value)
    {
        switch(thermostatMode)
        {
            case "auto": value = 0; return true;
            case "off": value = 1; return true;
            case "heat": value = 2; return true;
            case "emergency heat": value = 3; return true;
            case "cool": value = 4; return true;
        }

        value = -1;
        return false;
    }
}