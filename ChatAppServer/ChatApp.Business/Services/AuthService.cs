using ChatApp.Business.Interfaces;
using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using BCryptNet = BCrypt.Net.BCrypt;


namespace ChatApp.Business.Services;

public class AuthService(IUserRepository userRepo, TokenManager tokenManager) : IAuthService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly TokenManager _tokenManager = tokenManager;

    public async Task<UserEntity> RegisterAsync(string username, string password)
    {

        var existingUser = await _userRepo.ExistsAsync(u => u.Username == username);
        if (existingUser == true)
            throw new InvalidOperationException("Username already taken");
        var hash = HashPassword(password);
        var user = new UserEntity
        {
            Username = username,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow
        };
        await _userRepo.AddAsync(user);
        return user;
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _userRepo.GetAsync(u => u.Username == username) ?? throw new UnauthorizedAccessException("Invalid username");
        if(!VerifyPassword(password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid password");

        return _tokenManager.GenerateJwtToken(user);
    }

     
    private bool VerifyPassword(string password, string hash) => BCryptNet.Verify(password, hash);
    private string HashPassword(string password) => BCryptNet.HashPassword(password);
}
