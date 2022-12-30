using System;
using System.Diagnostics;

namespace hubitat2prom
{
    public static class ExceptionExtensions
    {
        private const string TRACE_DATETIME_FORMAT = "{0:[yyyy-MM-ddThh:mm:ss.fffK]} {1}";
        public static void TraceError(this Exception e)
            => Trace.TraceError(TRACE_DATETIME_FORMAT, DateTime.Now, e);
        public static void TraceError(this Exception e, string message)
        {
            Trace.TraceError(TRACE_DATETIME_FORMAT, DateTime.Now, message);
            TraceError(e);
        }
    }
}
