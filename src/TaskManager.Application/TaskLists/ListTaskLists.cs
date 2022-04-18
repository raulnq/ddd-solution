using Application;
using FluentValidation;

namespace TaskManager.Application.TaskLists
{
    public static class ListTaskLists
    {
        public class Query : ListQuery<Result>
        {
            public string? Name { get; set; }
        }

        public class Result
        {
            public int TaskListId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
        }

        public class QueryValidator : AbstractValidator<Query>
        {
        }
    }
}