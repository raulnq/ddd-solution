using Application;
using SqlKata.Execution;

namespace Infrastructure
{
    public abstract class SqlKataExistQueryRunner<TQuery> : SqlKataQueryRunner<TQuery, Any> where TQuery : Application.BaseQuery<Any>
    {
        protected SqlKataExistQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
        {
        }

        public override async Task<Any> Run(TQuery query)
        {
            var exist = await BuildQuery(query).ExistsAsync();

            return new Any() { Value = exist };
        }
    }
}