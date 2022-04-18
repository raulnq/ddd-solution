using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, string title, string version)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();

                options.CustomSchemaIds(CustomSchemaIdProvider.Get);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = title, Version = version });
            });

            return services;
        }
    }
}
