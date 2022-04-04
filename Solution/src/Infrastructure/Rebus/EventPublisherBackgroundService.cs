using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure
{
    public class EventPublisherBackgroundService : IHostedService
    {
        private static readonly ILogger _logger = Log.Logger.ForContext<EventPublisherBackgroundService>();
        private readonly RebusEventPublisher _eventPublisher;

        public EventPublisherBackgroundService(RebusEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Starting Event Publisher");

            _eventPublisher.Start();

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.Information("Stopping Event Publisher");

            return _eventPublisher.Stop();
        }
    }
}
