namespace Application
{
    public interface IQueryRunner<TQuery, TResult> : IQueryRunner where TQuery : BaseQuery<TResult>
    {
        Task<TResult> Run(TQuery query);
    }

    public interface IQueryRunner
    {
    }
}
