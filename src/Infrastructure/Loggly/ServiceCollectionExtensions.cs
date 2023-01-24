using Application;
using Domain;
using Loggly.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoggy(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Loggly");

            if (!config.Exists())
            {
                return services;
            }

            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var instance = LogglyConfig.Instance;

            instance.CustomerToken = config.GetValue("Token", "None");

            var applicationName = config.GetValue("ApplicationName", "None");

            instance.ApplicationName = applicationName;

            instance.Transport.EndpointHostname = config.GetValue("HostName", "None");

            instance.Transport.EndpointPort = 443;

            instance.Transport.LogTransport = LogTransport.Https;

            instance.TagConfig.Tags.Add(applicationName);

            if (!string.IsNullOrEmpty(env))
            {
                instance.TagConfig.Tags.Add(env);
            }

            return services;
        }
    }
}
