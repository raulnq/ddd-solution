using Application;
using Events;
using Rebus.Bus;
using Serilog;

namespace Infrastructure
{
    public class RebusEventPublisher : IEventPublisher
    {
        private readonly IBus _bus;

        private static readonly ILogger _logger = Log.Logger.ForContext<RebusEventPublisher>();

        public RebusEventPublisher(IBus bus)
        {
            _bus = bus;
        }

        public Task Publish(IEvent @event, IDictionary<string, string>? headers = null)
        {
            return Send(@event, headers);
        }

        private async Task Send(IEvent @event, IDictionary<string, string>? headers = null)
        {
            try
            {
                var eventTypeName = @event.GetType().Name;

                _logger.ForContext("Event", @event, true).Debug("Publishing event {eventTypeName}", eventTypeName);

                await _bus.Publish(@event, headers);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Publishing event");
            }
        }
    }
}
