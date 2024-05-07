using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Domain.Models;
using ToDo.Domain.Requests;
using ToDo.Infra.Persistence.Repositories.Interfaces;

namespace ToDo.Application.Controllers
{
    [ApiController]
    [Route("todos")]
    [Authorize]

    public class TodoController(ITodoRepository todoRepository, IUserRepository userRepository) : ControllerBase
    {
        protected readonly ITodoRepository TodoRepository = todoRepository;
        protected readonly IUserRepository UserRepository = userRepository;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var user = await UserRepository.GetAsNoTrackingAsync(User.Identity.Name);
            if (user is null)
            {
                return Unauthorized();
            }
            var todos = await TodoRepository.GetAllAsync(user.Id);
            return Ok(todos);
        }
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var todo = await TodoRepository.GetAsync(id);
            if (todo is null)
            {
                return NotFound(new
                {
                    Message = "ID não encontrado."
                });
            }
            return Ok(todo);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] EditTodoRequest model)
        {
            var todo = await TodoRepository.GetAsync(id);
            if (todo is null)
            {
                return NotFound(new
                {
                    Message = "ID não encontrado."
                });
            }
            if (model.Text is not null)
            {
                todo.Text = model.Text;
            }
            if (model.IsDone is bool isdone)
            {
                todo.IsDone = isdone;
            }
            await TodoRepository.UpdateAsync(todo);
            await TodoRepository.SaveAsync();
            return Ok(todo);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Deletar([FromRoute] Guid id)
        {
            var todo = await TodoRepository.GetAsync(id);
            if (todo is null)
            {
                return Ok();
            }
            await TodoRepository.DeleteAsync(todo);
            await TodoRepository.SaveAsync();
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTodoRequest model)
        {
            var user = await UserRepository.GetAsNoTrackingAsync(User.Identity.Name);
            if (user is null)
            {
                return Unauthorized();
            }
            var todo = new Todo
            {
                Text = model.Text,
                UserId = user.Id,
            };
            await TodoRepository.CreateAsync(todo);
            await TodoRepository.SaveAsync();
            return Ok();
        }
    }
}