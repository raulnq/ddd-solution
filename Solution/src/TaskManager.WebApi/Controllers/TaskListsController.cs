using Application;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Application.TaskLists;

namespace TaskManager.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TaskListsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TaskListsController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public Task<RegisterTaskList.Result> RegisterTaskList([FromBody] RegisterTaskList.Command command) => _mediator.Send(command);

        [HttpGet]
        public Task<ListResults<ListTaskLists.Result>> ListTaskLists([FromQuery] ListTaskLists.Query query) => _mediator.Send(query);
    }
}