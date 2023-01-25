using System.Net;
using TaskManager.Application.TaskLists;
using Bogus;
using Shouldly;
using Tests;
using Application;

namespace TaskManager.Tests
{
    internal class TaskListDsl
    {
        private readonly HttpDriver _httpDriver;

        public TaskListDsl(HttpDriver httpDriver)
        {
            _httpDriver = httpDriver;
        }

        public async Task<(RegisterTaskList.Command, RegisterTaskList.Result?)> Register(Action<RegisterTaskList.Command>? setup = null, string? errorMesage = null)
        {
            var faker = new Faker<RegisterTaskList.Command>()
                .RuleFor(command => command.Description, faker => faker.Lorem.Sentence())
                .RuleFor(command => command.Name, faker => faker.Lorem.Word());

            var request = faker.Generate();

            setup?.Invoke(request);

            var (status, result, error) = await _httpDriver.Post<RegisterTaskList.Command, RegisterTaskList.Result>("TaskLists", request);

            (status, result, error).Check(errorMesage, assert: result =>
            {
                result.TaskListId.ShouldBeGreaterThan(0);
            });

            return (request, result);
        }

        public async Task<(ListTaskLists.Query, ListResults<ListTaskLists.Result>?)> List(Action<ListTaskLists.Query>? setup = null, string? errorMesage = null)
        {
            var faker = new Faker<ListTaskLists.Query>()
                .RuleFor(command => command.Name, faker => faker.Lorem.Word());

            var request = faker.Generate();

            setup?.Invoke(request);

            var (status, result, error) = await _httpDriver.Get<ListTaskLists.Query, ListResults<ListTaskLists.Result>>("TaskLists", request);

            (status, result, error).Check(errorMesage, assert: result =>
            {
                result.TotalCount.ShouldBeGreaterThan(0);
            });

            return (request, result);
        }
    }
}