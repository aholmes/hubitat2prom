using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace hubitat2prom.Tests;

public abstract class TestBase
{
    protected MockCreator _mockCreator;
    protected HubitatEnv _env;
    protected Lazy<Assembly> _assembly = new Lazy<Assembly>(() => typeof(hubitat2prom.Tests.TestHubitat).Assembly);

    public TestBase(MockCreator mockCreator)
    {
        _mockCreator = mockCreator;
        _env = new HubitatEnv(new Uri("http://example.org"), Guid.NewGuid());
    }

    protected string _readResourceJson(string resourceName)
    {
        var resourceStream = _assembly.Value.GetManifestResourceStream($"{_assembly.Value.GetName().Name}.Data.{resourceName}.json")!;
        using (var streamReader = new StreamReader(resourceStream)) return streamReader.ReadToEnd();
    }
}
