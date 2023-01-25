using Tests;

namespace TaskManager.Tests
{
    internal class AppDsl : IAsyncDisposable
    {
        private readonly ApplicationFactory _applicationFactory;

        public AppDsl()
        {
            var clock = new FixedClock(DateTimeOffset.UtcNow);

            _applicationFactory = new ApplicationFactory
            {
                Clock = clock
            };

            var httpDriver = new HttpDriver(_applicationFactory);

            TaskList = new TaskListDsl(httpDriver);
        }

        public TaskListDsl TaskList { get; }

        public ValueTask DisposeAsync()
        {
            return _applicationFactory.DisposeAsync();
        }
    }
}