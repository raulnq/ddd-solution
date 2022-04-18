using Domain;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskManagerInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructure(configuration, typeof(ServiceCollectionExtensions).Assembly);

            services.AddPersistance<ApplicationDbContext>(configuration);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddBus(configuration, typeof(ServiceCollectionExtensions).Assembly);

            return services;
        }
    }
}