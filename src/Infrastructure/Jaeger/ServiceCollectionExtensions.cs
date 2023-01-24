using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Jaeger");

            if (!config.Exists())
            {
                return services;
            }

            var port = config.GetValue("Port", 6831);

            var host = config.GetValue("Host", "localhost");

            var serviceName = config.GetValue("ServiceName", "None");

            var serviceVersion = config.GetValue("ServiceVersion", "1.0.0");

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddJaegerExporter(o =>
                {
                    o.AgentPort = port;
                    o.AgentHost = host;
                })
                .AddSource(serviceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: serviceName, serviceVersion: serviceVersion))
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddAspNetCoreInstrumentation();
            });

            return services;
        }
    }
}
