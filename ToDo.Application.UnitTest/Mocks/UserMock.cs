using Bogus;
using ToDo.Domain.Models;

namespace ToDo.Application.UnitTest.Mocks
{
    public static class UserMock
    {
        public static User Mock() 
        {
            const string hash = "$argon2id$v=19$m=65536,t=3,p=1$Gl6h8/b1egX3vPY7mpTWgw$3dTQHg20++N+zp5edojnU73mLyJJbgs1yeqs7I+v+c0";
            var user = new Faker<User>()
                .RuleFor(u => u.Name, f => f.Person.FullName)
                .RuleFor(u => u.Email, f => f.Person.Email)
                .RuleFor(u => u.Password, hash);
            return user.Generate();       
        }
    }
}
