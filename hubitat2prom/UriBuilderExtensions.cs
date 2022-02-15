using System;
using System.Linq;

namespace hubitat2prom;

public static class UriBuilderExtensions
{
    /**
    * Changes the UriBuilder.Path to include additional path
    * segments at the end of the existing Path.
    */
    public static void PathAppend(this UriBuilder uriBuilder, params string[] segments)
    {
        uriBuilder.Path = string.Join('/', new[] { uriBuilder.Path.TrimEnd('/') }
            .Concat(segments.Select(s => s.Trim('/'))));
    }
}