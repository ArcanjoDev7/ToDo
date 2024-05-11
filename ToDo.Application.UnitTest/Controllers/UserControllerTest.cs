using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.Application.Controllers;
using ToDo.Application.UnitTest.Mocks;
using ToDo.Domain.Models;
using ToDo.Domain.Requests;
using ToDo.Infra.Persistence.Repositories.Interfaces;
using ToDo.Service.Services.Interfaces;

namespace ToDo.Application.UnitTest.Controllers
{
    public class UserControllerTest
    {
        protected readonly IUserRepository UserRepository;
        protected readonly User User;
        protected readonly ITokenService TokenService;
        protected readonly UserController UserController;
        public UserControllerTest()
        { 
            UserRepository = A.Fake<IUserRepository>();
            User = UserMock.Mock();
            A.CallTo(() => UserRepository.GetAsync(User.Email)).Returns(User);
            A.CallTo(() => UserRepository.GetAsNoTrackingAsync(User.Email)).Returns(User);
            UserController = new UserController(UserRepository);
        }
        [Fact]
        public async Task Create_ShouldCreateAnUser()
        {
            // Arrange
            A.CallTo(()=> UserRepository.EmailExistAsync(User.Email)).Returns(false);
            var body = new CreateUserRequest
            {
                Email = User.Email,
                Name = User.Name,
                Password = "123",
            };
            // Act
            var result = await UserController.Create(body);
            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<CreatedResult>();
        }
        [Fact]
        public async Task Login_ShouldLoginAnUser()
        {
            // Arrange
            var fakeUserRepository = A.Fake<IUserRepository>();
            var fakeTokenService = A.Fake<ITokenService>();
            var fakeUser = new User
            {
                Email = User.Email,
                Name = User.Name,
                Password = "123",
            };
            A.CallTo(() => fakeUserRepository.GetAsNoTrackingAsync("miguel@exemplo.com"))
                .Returns(fakeUser);
            A.CallTo(() => fakeTokenService.GetToken(A<User>.Ignored))
                .Returns("expected_token");
            A.CallTo(() => UserRepository.GetAsNoTrackingAsync(User.Email)).Returns(User);
            var body = new LoginRequest("miguelarcanjodearaujocosta@gmail.com","123")
            {
                Email = User.Email,
                Password = "123",
            };
            // Act
            var result = await UserController.Login(body, fakeTokenService);
            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("expected_token", okResult.Value);
        }
        [Fact]
        public async Task Get_ShouldGetAnUser()
        {
            // Arrange
            var fakeUserRepository = A.Fake<IUserRepository>();
            var user = new User
            {
                Email = User.Email,
                Name = User.Name,
                Password = "123",
            };
            // Configurar o repositório para retornar o usuário quando solicitado
            A.CallTo(() => fakeUserRepository.GetAsNoTrackingAsync("Miguel"))
                .Returns(Task.FromResult(user));
            var userController = new UserController(fakeUserRepository);

            // Criar um contexto HTTP com uma identidade simulada
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[]
                    {
                    new Claim(ClaimTypes.Name, "Miguel")
                    },
                    "TestAuthType"
                )
            );
            userController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            // Act
            var result = await userController.Get();

            // Assert
            Assert.IsType<OkObjectResult>(result); 
            var okResult = result as OkObjectResult;
            Assert.Equal(user, okResult.Value);
        }
        [Fact]
        public async Task Delete_ShouldDeleteAnUser() 
        {
            // Arrange
            var fakeUserRepository = A.Fake<IUserRepository>();
            var user = new User
            {
                Email = User.Email,
                Name = User.Name,
                Password = "123",
            };
            // Simular que o usuário é encontrado
            A.CallTo(() => fakeUserRepository.GetAsNoTrackingAsync("Miguel"))
                .Returns(Task.FromResult(user));
            var userController = new UserController(fakeUserRepository);
            // Simular o contexto HTTP para User.Identity.Name
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(
                new ClaimsIdentity(
                    new Claim[] { new Claim(ClaimTypes.Name, "Miguel") },
                    "TestAuthType"
                )
            );
            userController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            // Act
            var result = await userController.Deletar(); 
            // Assert
            A.CallTo(() => fakeUserRepository.DeleteAsync(user)).MustHaveHappened();
            A.CallTo(() => fakeUserRepository.SaveAsync()).MustHaveHappened();

        }
    }

}
