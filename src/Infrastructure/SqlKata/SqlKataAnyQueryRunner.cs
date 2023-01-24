using Application;
using SqlKata.Execution;

namespace Infrastructure
{
    public abstract class SqlKataAnyQueryRunner<TQuery> : SqlKataQueryRunner<TQuery, Any> where TQuery : BaseQuery<Any>
    {
        protected SqlKataAnyQueryRunner(QueryFactory queryFactory, DbSchema dbSchema) : base(queryFactory, dbSchema)
        {
        }

        public override async Task<Any> Run(TQuery query)
        {
            var exist = await BuildQuery(query).ExistsAsync();

            return new Any() { Value = exist };
        }
    }
}