﻿using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Infrastructure
{
    public class AzureServiceBusTelemetryProcessorFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        public AzureServiceBusTelemetryProcessorFilter(ITelemetryProcessor next)
        {
            this.Next = next;
        }

        public void Process(ITelemetry item)
        {
            if (IsAzureServiceBus(item)) { return; }

            this.Next.Process(item);
        }

        private bool IsAzureServiceBus(ITelemetry item)
        {
            var dependency = item as DependencyTelemetry;

            return dependency?.Type == "Azure Service Bus" && dependency?.Name == "Receive";
        }
    }
}
