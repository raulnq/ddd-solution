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
            var infrastructureConfiguration = configuration.GetSection("Infrastructure");

            services.AddOptions();

            services.AddHttpContextAccessor();

            services.AddHealthChecks();

            services.AddProblemDetails(options => ConfigureProblemDetails(options));

            services.AddAuthentication(infrastructureConfiguration);

            services.AddCors(infrastructureConfiguration);

            services.AddSwagger(infrastructureConfiguration);

            services.AddTracing(infrastructureConfiguration);

            services.AddLoggy(infrastructureConfiguration);

            services.AddMemoryCache();

            services.AddApplicationInsights(configuration);

            return services;
        }

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
        {
            services.AddQueryRunners(configuration, assembly);

            services.AddRebus(configuration, assembly);

            services.AddLocalization(assembly);

            return services;
        }
    }
}
