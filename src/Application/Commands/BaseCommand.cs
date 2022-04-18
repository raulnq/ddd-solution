using MediatR;

namespace Application
{
    public abstract class BaseCommand : ICommand, IRequest
    {
    }

    public abstract class BaseCommand<TResult> : ICommand, IRequest<TResult>
    {
    }
}