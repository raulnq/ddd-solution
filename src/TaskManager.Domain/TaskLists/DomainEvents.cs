using Domain;

namespace TaskManager.Domain.TaskLists
{
    public static class DomainEvents
    {
        public class TaskListRegistered : IDomainEvent
        {
            public int TaskListId { get; set; }
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
        }
    }
}