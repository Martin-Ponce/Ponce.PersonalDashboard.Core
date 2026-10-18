using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace Ponce.PersonalDashboard.Core.API.Helpers.Telemetry
{
    public class TraceEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            var activity = Activity.Current;
            if (activity != null)
            {
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("TraceId", activity.TraceId.ToString()));
                logEvent.AddOrUpdateProperty(propertyFactory.CreateProperty("SpanId", activity.SpanId.ToString()));
            }
        }
    }
}
