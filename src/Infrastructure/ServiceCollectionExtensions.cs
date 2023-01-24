using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Hellang.Middleware.ProblemDetails;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            services.AddLocalization();

            services.AddProblemDetails(options => ConfigureProblemDetails(options));

            services.AddCors(configuration);

            services.AddSwagger(configuration);

            services.AddTracing(configuration);

            services.AddLoggy(configuration);

            services.AddMemoryCache();

            services.AddApplicationInsights(configuration);

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddQueryRunners(configuration, assembly);

            services.AddRebus(configuration, assembly);

            return services;
        }
    }
}
