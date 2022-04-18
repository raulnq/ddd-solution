using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddBehaviors();

            return services;
        }

        public static IServiceCollection AddApplication(this IServiceCollection services, params Assembly[] assemblies)
        {
            services.AddAutoMapper(assemblies);

            services.AddValidatorsFromAssemblies(assemblies);

            services.AddMediatR(assemblies);

            services.AddQueryHandlers(assemblies);

            return services;
        }
    }
}
