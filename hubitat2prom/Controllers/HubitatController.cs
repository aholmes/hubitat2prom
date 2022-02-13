using System;
using System.Collections.Generic;
using System.Linq;
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
    public async Task<InfoModel> GetInfo()//IEnumerable<int> Get()
    {
        var devices = await _hubitat.Devices();
        return null;
        //return Enumerable.Range(1, 5).Select(index => new Hubitat
        //{
        //    Date = DateTime.Now.AddDays(index),
        //    TemperatureC = rng.Next(-20, 55),
        //    Summary = Summaries[rng.Next(Summaries.Length)]
        //})
        //.ToArray();
    }
}