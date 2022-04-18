using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;
using System.Reflection;
using Hellang.Middleware.ProblemDetails;
using OpenTelemetry.Trace;
using OpenTelemetry.Resources;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string applicationName, string applicationVersion)
        {
            services.AddOptions();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            services.AddLocalization();

            services.AddProblemDetails(options => ConfigureProblemDetails(options));

            services.AddCors(configuration);

            services.AddSwagger(applicationName, applicationVersion);

            services.AddTracing(configuration, applicationName, applicationVersion);

            services.AddMemoryCache();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddQueryRunners(configuration, assembly);

            return services;
        }

        public static IServiceCollection AddTracing(this IServiceCollection services, IConfiguration configuration, string applicationName, string applicationVersion)
        {
            var settings = configuration.GetSection("Jaeger").Get<JaegerSettings>();

            if (settings == null)
            {
                return services;
            }

            services.AddOpenTelemetryTracing(builder =>
            {
                builder.AddJaegerExporter(o =>
                {
                    o.AgentPort = settings.Port;
                    o.AgentHost = settings.Host;
                })
                .AddSource(applicationName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: applicationName, serviceVersion: applicationVersion))
                .AddHttpClientInstrumentation()
                .AddSqlClientInstrumentation()
                .AddAspNetCoreInstrumentation();
            });

            return services;
        }

        private static IServiceCollection AddCors(this IServiceCollection services, IConfiguration configuration)
        {
            var corsSection = configuration.GetSection("Cors");

            if (corsSection == null)
            {
                return services;
            }

            return services.AddCors(options =>
            {
                foreach (var item in corsSection.GetChildren())
                {
                    var origins = item.Get<string[]>();

                    options.AddPolicy(item.Key, builder => builder.WithOrigins(origins).AllowAnyMethod().AllowAnyHeader());
                }
            });
        }
    }
}
