using Application;
using Domain;
using FluentValidation;
using MediatR;
using TaskManager.Domain.TaskLists;

namespace TaskManager.Application.TaskLists
{
    public static class RegisterTaskList
    {
        public class Command : BaseCommand<Result>
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }

        public class Result
        {
            public int TaskListId { get; set; }
        }

        public class CommandValidator : AbstractValidator<Command>
        {
            public CommandValidator()
            {
                RuleFor(command => command.Name).MaximumLength(255).NotEmpty();
                RuleFor(command => command.Description).MaximumLength(255);
            }
        }

        public class CommandHandler : IRequestHandler<Command, Result>
        {
            private readonly IRepository<TaskList> _taskListRepository;

            public CommandHandler(IRepository<TaskList> taskListRepository)
            {
                _taskListRepository = taskListRepository;
            }

            public async Task<Result> Handle(Command command, CancellationToken cancellationToken)
            {
                var taskListId = await _taskListRepository.GetNextValue();

                var role = new TaskList(new TaskListId(taskListId), command.Name!, command.Description);

                _taskListRepository.Add(role);

                return new Result() { TaskListId = taskListId };
            }
        }
    }
}