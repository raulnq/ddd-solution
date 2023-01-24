using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationInsights(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("ApplicationInsights");

            if (!config.Exists()) return services;

            var roleName = config["RoleName"];

            services.AddApplicationInsightsTelemetry();

            services.AddSingleton<ITelemetryInitializer>(_ => new CloudRoleNameTelemetryInitializer(roleName));

            services.AddSingleton<ITelemetryInitializer, UserTelemetryInitializer>();

            services.AddApplicationInsightsTelemetryProcessor<AzureServiceBusTelemetryProcessorFilter>();

            services.AddApplicationInsightsTelemetryProcessor<LoggyTelemetryProcessorFilter>();

            services.AddApplicationInsightsTelemetryProcessor<SentryTelemetryProcessorFilter>();

            var enableSqlCommandTextInstrumentation = config.GetValue("EnableSqlCommandTextInstrumentation", false);

            services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) => module.EnableSqlCommandTextInstrumentation = enableSqlCommandTextInstrumentation);

            return services;
        }
    }
}
