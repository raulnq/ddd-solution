using Domain;

namespace Application
{
    public interface IDomainEventSource
    {
        public IEnumerable<IDomainEvent> Get();
    }
}
