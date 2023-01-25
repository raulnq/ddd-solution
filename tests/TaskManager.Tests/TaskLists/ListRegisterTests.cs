namespace TaskManager.Tests.TaskLists
{
    public class ListRegisterTests : IAsyncLifetime
    {
        private readonly AppDsl _appDsl;

        public ListRegisterTests()
        {
            _appDsl = new AppDsl();
        }

        public Task DisposeAsync()
        {
            return _appDsl.DisposeAsync().AsTask();
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        [Fact]
        public async Task list_should_be_ok()
        {
            var (command, result) = await _appDsl.TaskList.Register();

            await _appDsl.TaskList.List(query => query.Name = command.Name);
        }
    }
}