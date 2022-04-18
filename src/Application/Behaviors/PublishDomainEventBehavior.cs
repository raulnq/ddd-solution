using MediatR;

namespace Application
{
    public class PublishDomainEventBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        private readonly IDomainEventSource _source;
        private readonly IMediator _mediator;

        public PublishDomainEventBehavior(
            IDomainEventSource source,
            IMediator mediator)
        {
            _source = source;
            _mediator = mediator;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();

            foreach (var @event in _source.Get())
            {
                await _mediator.Publish(@event);
            }

            return response;
        }
    }
}
