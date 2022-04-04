using MediatR;

namespace Application
{
    public class BaseQuery<TResult> : IQuery, IRequest<TResult>
    {
    }
}
