using Application;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Domain.TaskLists;
using TaskManager.Events.TaskLists;

namespace TaskManager.Application.TaskLists
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddTaskLists(this IServiceCollection services)
        {
            services.AddEventDispatcher<DomainEvents.TaskListRegistered, TaskListRegistered>();

            return services;
        }
    }
}