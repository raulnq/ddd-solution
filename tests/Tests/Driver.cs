namespace Tests
{
    public abstract class Driver
    {
        protected readonly TimeoutMonitor _timeoutMonitor;
        protected readonly static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);
        protected readonly static TimeSpan _defaultInterval = TimeSpan.FromSeconds(30);
        protected Driver()
        {
            _timeoutMonitor = new TimeoutMonitor();
        }

        protected Task WaitFor(Func<Task> taskFactory, TimeSpan timeout, TimeSpan interval) => _timeoutMonitor.RunUntil(taskFactory, timeout, interval);
    }
}