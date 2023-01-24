using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace Infrastructure
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var config = configuration.GetSection("Swagger");

            if (!config.Exists())
            {
                return services;
            }

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(options =>
            {
                options.DescribeAllParametersInCamelCase();

                options.CustomSchemaIds(CustomSchemaIdProvider.Get);

                options.SwaggerDoc("v1", new OpenApiInfo { Title = config.GetValue("Title", "None"), Version = config.GetValue("Version", "1.0.0") });
            });

            return services;
        }
    }

    public static partial class AppBuilderExtensions
    {
        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
        {
            var config = configuration.GetSection("Swagger");

            if (!config.Exists())
            {
                return app;
            }

            app.UseSwagger();

            app.UseSwaggerUI(c => c.SwaggerEndpoint(config.GetValue("Url", "/swagger/v1/swagger.json"), config.GetValue("Title", "None")));

            return app;
        }
    }
}
