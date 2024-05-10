using FakeItEasy;
using FakeItEasy.Sdk;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using ToDo.Application.Controllers;
using ToDo.Application.UnitTest.Mocks;
using ToDo.Domain.Models;
using ToDo.Domain.Requests;
using ToDo.Infra.Persistence.Repositories.Interfaces;

namespace ToDo.Application.UnitTest.Controllers
{
    public class UserControllerTest
    {
        protected readonly IUserRepository UserRepository;
        protected readonly User User; 
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
    }
}
