using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application.TaskLists
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskLists(this IServiceCollection services)
        {
            return services;
        }
    }
}