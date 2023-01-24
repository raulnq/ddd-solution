using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Infrastructure
{
    public class SentryTelemetryProcessorFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public SentryTelemetryProcessorFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (IsSentry(item)) { return; }

            this.Next.Process(item);
        }

        private bool IsSentry(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;

            return dependency?.Type == "Http" && dependency?.Target?.Contains("sentry") == true;
        }
    }
}
