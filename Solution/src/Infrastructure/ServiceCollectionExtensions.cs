using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Execution;
using System.Reflection;
using Hellang.Middleware.ProblemDetails;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string applicationName)
        {
            services.AddOptions();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            services.AddLocalization();

            services.AddProblemDetails(options => ConfigureProblemDetails(options));

            services.AddCors(configuration);

            services.AddSwagger(applicationName, "v1");

            services.AddMemoryCache();

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddQueryRunners(configuration, assembly);

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
