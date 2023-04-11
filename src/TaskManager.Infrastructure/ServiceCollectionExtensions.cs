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
            var infrastructureConfiguration = configuration.GetSection("Infrastructure");

            services.AddInfrastructure(configuration, typeof(ServiceCollectionExtensions).Assembly);

            services.AddPersistance<ApplicationDbContext>(infrastructureConfiguration);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }
    }
}