using Application;
using Events;
using Rebus.Bus;
using Serilog;
using System.Threading.Tasks.Dataflow;

namespace Infrastructure
{
    public class RebusEventPublisher : IEventPublisher
    {
        private readonly IBus _bus;

        private static readonly ILogger _logger = Log.Logger.ForContext<RebusEventPublisher>();

        private ActionBlock<IEvent>? _actionBlock;

        public RebusEventPublisher(IBus bus)
        {
            _bus = bus;
        }

        public Task Publish(IEvent @event)
        {
            return _actionBlock!.SendAsync(@event);
        }

        public Task Stop()
        {
            _actionBlock!.Complete();

            return _actionBlock.Completion;
        }

        public void Start()
        {
            _actionBlock = new ActionBlock<IEvent>(@event => Send(@event), new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount,
            });
        }

        private async Task Send(IEvent @event)
        {
            try
            {
                var eventTypeName = @event.GetType().Name;

                _logger.ForContext("Event", @event, true).Debug("Publishing event {eventTypeName}", eventTypeName);

                await _bus.Publish(@event);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error Publishing event");
            }
        }
    }
}
