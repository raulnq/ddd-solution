using Infrastructure;
using SqlKata;
using SqlKata.Execution;
using TaskManager.Application.TaskLists;

namespace TaskManager.Infrastructure.TaskLists
{
    public class ListTaskListsQueryRunner : SqlKataListQueryRunner<ListTaskLists.Query, ListTaskLists.Result>
    {
        private readonly Table _taskLists;

        public ListTaskListsQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
        {
            _taskLists = new Table(dbSchema.Name, "TaskLists");
        }

        protected override Query BuildQuery(ListTaskLists.Query query)
        {
            var statement = QueryFactory.Query(_taskLists);

            if (!string.IsNullOrEmpty(query.Name))
            {
                statement = statement.WhereLike(_taskLists.Field("Name"), $"%{query.Name}%");
            }

            return statement;
        }
    }
}
