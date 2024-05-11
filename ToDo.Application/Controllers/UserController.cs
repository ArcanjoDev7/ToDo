using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDo.Domain.Models;
using ToDo.Domain.Requests;
using ToDo.Infra.Persistence.Repositories.Interfaces;
using ToDo.Service.Services.Interfaces;

namespace ToDo.Application.Controllers
{
    [ApiController]
    [Route("users")]
    public class UserController(IUserRepository userRepository) : ControllerBase
    {
        protected readonly IUserRepository UserRepository = userRepository;
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest model)
        {
                
            if(await UserRepository.EmailExistAsync(model.Email))
            {
                return BadRequest(new
                {
                    Message = "O email ja esta em uso."
                });
            }
            var user = new User
            {
                Email = model.Email,
                Name = model.Name,
                Password = Argon2.Hash(model.Password)
            };
            await UserRepository.CreateAsync(user);
            await UserRepository.SaveAsync();
            return Created("", user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model, [FromServices] ITokenService tokenService)
        {
            var user = await UserRepository.GetAsNoTrackingAsync(model.Email);
            if (user is null)
            {
                return BadRequest(new
                {
                    Message = "O usuario não foi encontrado."
                });
            }
            if (!Argon2.Verify(user.Password, model.Password))
            {
                return BadRequest(new
                {
                    Message = "Senha Invalida."
                });
            }
            return Ok(tokenService.GetToken(user));
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var user = await UserRepository.GetAsNoTrackingAsync(User.Identity.Name);
            if (user is null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> Deletar()
        {
            var user = await UserRepository.GetAsNoTrackingAsync(User.Identity.Name);
            if (user is null)
            {
                return Ok();
            }
            await UserRepository.DeleteAsync(user);
            await UserRepository.SaveAsync();
            return Ok();
        }

    }
}
