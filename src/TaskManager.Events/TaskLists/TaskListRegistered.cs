using Events;

namespace TaskManager.Events.TaskLists
{
    public class TaskListRegistered : @Event
    {
        public int TaskListId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
    }
}