using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
}