using Application;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Application.TaskLists;

namespace TaskManager.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskManagerApplication(this IServiceCollection services)
        {
            services.AddApplication(typeof(ServiceCollectionExtensions).Assembly);

            services.AddTaskLists();

            return services;
        }
    }
}
