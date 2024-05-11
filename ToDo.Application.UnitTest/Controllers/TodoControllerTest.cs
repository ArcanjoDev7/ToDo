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

namespace ToDo.Application.UnitTest.Controllers
{
    public class TodoControllerTest
    {
        protected readonly ITodoRepository TodoRepository;
        protected readonly IUserRepository UserRepository;
        protected readonly Todo Todo;
        protected readonly User User;
        protected readonly TodoController TodoController;
        public TodoControllerTest()
        {
            TodoRepository = A.Fake<ITodoRepository>();
            UserRepository = A.Fake<IUserRepository>();
            Todo = TodoMock.Mock();
            A.CallTo(() => UserRepository.GetAsNoTrackingAsync("Miguel")).Returns(Task.FromResult(User));
            A.CallTo(() => TodoRepository.GetAsync(Todo.Id)).Returns(Todo);
            A.CallTo(() => TodoRepository.GetAllAsync(Todo.UserId)).Returns(new List<Todo> { Todo });
            TodoController = new TodoController(TodoRepository, UserRepository);
        }
        [Fact]
        public async Task Get_ShouldGetAnTodo()
        {
            // Arrange
            var fakeUserRepository = A.Fake<IUserRepository>();
            var fakeTodoRepository = A.Fake<ITodoRepository>();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "miguel@example.com",
                Name = "Miguel",
                Password = "123",

            };
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Text = "Sample todo",
                IsDone = false
            };
            A.CallTo(() => fakeUserRepository.GetAsNoTrackingAsync("Miguel"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => fakeTodoRepository.GetAllAsync(user.Id))
                .Returns(new List<Todo> { todo });

            var todoController = new TodoController(fakeTodoRepository, fakeUserRepository);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "Miguel")
            }, "TestAuthentication"));

            todoController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var todos = await todoController.Get(user.Id);

            // Assert
            Assert.IsType<OkObjectResult>(todos);
            var okResult = todos as OkObjectResult;
        }
        [Fact]
        public async Task Get_ShouldGetGuidAnTodo()
        {
            // Arrange
            A.CallTo(() => TodoRepository.GetAsync(Todo.Id));

            // Act
            var result = await TodoController.Get(Todo.Id);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;

        }
        [Fact]
        public async Task Put_ShouldPutAnTodo()
        {
            var fakeTodoRepository = A.Fake<ITodoRepository>();
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Text = "Sample todo",
                IsDone = false
            };
            A.CallTo(() => fakeTodoRepository.GetAsync(todo.Id)).Returns(todo);
            var editRequest = new EditTodoRequest
            {
                Text = "Updated text",
                IsDone = true
            };
            var todoController = new TodoController(fakeTodoRepository, A.Fake<IUserRepository>());

            // Act
            var result = await todoController.Put(todo.Id, editRequest);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);

            var okObjectResult = result as OkObjectResult;
            Assert.NotNull(okObjectResult.Value);

            var updatedTodo = okObjectResult.Value as Todo;
            Assert.NotNull(updatedTodo);
            Assert.Equal(todo.Id, updatedTodo.Id);
            Assert.Equal(editRequest.Text, updatedTodo.Text);
            Assert.Equal(editRequest.IsDone, updatedTodo.IsDone);

            A.CallTo(() => fakeTodoRepository.UpdateAsync(todo)).
                MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTodoRepository.SaveAsync())
                .MustHaveHappenedOnceExactly();
        }
        [Fact]
        public async Task DeleteTodo_ShouldDeleteTodoAnTodo()
        {
            // Arrange
            var fakeTodoRepository = A.Fake<ITodoRepository>();
            var todoId = Guid.NewGuid();
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Text = "Sample todo",
                IsDone = false
            };
            A.CallTo(() => fakeTodoRepository.GetAsync(todoId)).Returns(todo);
            var todoController = new TodoController(fakeTodoRepository, A.Fake<IUserRepository>());

            // Act
            var result = await todoController.Get(todoId);

            // Assert
            result.Should().NotBeNull().And.BeOfType<OkObjectResult>();
            var okObjectResult = result as OkObjectResult;
            okObjectResult.Value.Should().NotBeNull().And.BeAssignableTo<Todo>();
            var deleteResult = await todoController.Deletar(todoId);
            deleteResult.Should().NotBeNull().And.BeOfType<OkResult>();
            A.CallTo(() => fakeTodoRepository.DeleteAsync(todo)).MustHaveHappenedOnceExactly();
            A.CallTo(() => fakeTodoRepository.SaveAsync()).MustHaveHappenedOnceExactly();
        }
        [Fact]
        public async Task CreateTodo_ShouldCreateTodoAnTodo()
    {
            // Arrange
            var fakeUserRepository = A.Fake<IUserRepository>();
            var fakeTodoRepository = A.Fake<ITodoRepository>();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "miguel@example.com",
                Name = "Miguel",
                Password = "123",

            };
            var todo = new Todo
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Text = "Sample todo",
                IsDone = false
            };
            var todoList = new List<Todo>();
            A.CallTo(() => fakeUserRepository.GetAsNoTrackingAsync("Miguel"))
                .Returns(Task.FromResult(user));
            A.CallTo(() => fakeTodoRepository.GetAllAsync(user.Id))
                .Returns(todoList);

            var todoController = new TodoController(fakeTodoRepository, fakeUserRepository);
            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.Name, "Miguel")
            }, "TestAuthentication"));

            todoController.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
            var body = new CreateTodoRequest("TExt")
            {
                Text = string.Empty
            };
            // Act
            var result = await todoController.Create(body);

            // Assert
            result.Should().NotBeNull();
        }

    }
}
