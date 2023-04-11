using Infrastructure;
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
                Clock = clock,
                ApiKeys = new ApiKeySettings { { "fake-api-key", "Admin" } }
            };

            var httpDriver = new HttpDriver(_applicationFactory, "fake-api-key");

            TaskList = new TaskListDsl(httpDriver);
        }

        public TaskListDsl TaskList { get; }

        public ValueTask DisposeAsync()
        {
            return _applicationFactory.DisposeAsync();
        }
    }
}