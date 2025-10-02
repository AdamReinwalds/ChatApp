using ChatApp.Data.Entities;

namespace ChatApp.Business.Interfaces;

public interface IAuthService
{
    Task<string> LoginAsync(string username, string password);
    Task<UserEntity> RegisterAsync(string username, string password);
}
