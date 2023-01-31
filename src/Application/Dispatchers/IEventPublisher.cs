using Events;

namespace Application
{
    public interface IEventPublisher
    {
        Task Publish(IEvent @event, IDictionary<string, string>? headers = null);
    }
}