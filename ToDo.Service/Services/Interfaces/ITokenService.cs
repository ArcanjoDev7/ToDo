using ToDo.Domain.Models;

namespace ToDo.Service.Services.Interfaces
{
    public interface ITokenService
    {
       string GetToken(User user);
    }
}
