using Domain;
using static TaskManager.Domain.TaskLists.DomainEvents;

namespace TaskManager.Domain.TaskLists
{
    public record TaskListId(int Value);

    public class TaskList : AggregateRoot
    {
        public TaskListId TaskListId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }

        private TaskList()
        {

        }

        public TaskList(TaskListId id, string name, string? description)
        {
            Name = name;
            Description = description;
            TaskListId = id;
            AddEvent(new TaskListRegistered()
            {
                TaskListId = TaskListId.Value,
                Name = Name,
                Description = Description
            });
        }
    }
}