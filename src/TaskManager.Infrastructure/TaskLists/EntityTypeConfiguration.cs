using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Domain.TaskLists;

namespace TaskManager.Infrastructure.TaskLists
{
    public class EntityTypeConfiguration : IEntityTypeConfiguration<TaskList>
    {
        public void Configure(EntityTypeBuilder<TaskList> builder)
        {
            builder
                .ToTable("TaskLists");

            builder
                .HasKey(taskList => taskList.TaskListId);

            builder
                .Property(taskList => taskList.TaskListId)
                .HasConversion(taskListId => taskListId.Value, value => new TaskListId(value));
        }
    }
}
