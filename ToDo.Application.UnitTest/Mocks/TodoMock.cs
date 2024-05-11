using Bogus;
using ToDo.Domain.Models;

namespace ToDo.Application.UnitTest.Mocks
{
    public class TodoMock
    {
        public static Todo Mock()
        {
            var todo = new Faker<Todo>()
                .RuleFor(u => u.Id, f => f.Random.Guid())
                .RuleFor(u => u.UserId, f => f.Random.Guid())
                .RuleFor(u => u.Text, f => f.Lorem.Sentence())
                .RuleFor(u => u.IsDone, f => f.Random.Bool());
                return todo.Generate();
        }           
    }
}
