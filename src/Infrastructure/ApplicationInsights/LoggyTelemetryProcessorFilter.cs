using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Infrastructure
{
    public class LoggyTelemetryProcessorFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public LoggyTelemetryProcessorFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (IsLoggy(item)) { return; }

            this.Next.Process(item);
        }

        private bool IsLoggy(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;

            return dependency?.Type == "Http" && dependency?.Target?.Contains("loggly") == true;
        }
    }
}
