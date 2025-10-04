using ChatApp.Business.Interfaces;
using ChatApp.Business.Results;
using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using BCryptNet = BCrypt.Net.BCrypt;


namespace ChatApp.Business.Services;

public class AuthService(IUserRepository userRepo, TokenManager tokenManager) : IAuthService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly TokenManager _tokenManager = tokenManager;

    public async Task<ServiceResult> RegisterAsync(string username, string password)
    {

        var existingUser = await _userRepo.ExistsAsync(u => u.Username == username);
        if (existingUser == true)
            return new ServiceResult { Success = false, Message = "Username already exists" };
        var hash = HashPassword(password);
        var user = new UserEntity
        {
            Username = username,
            PasswordHash = hash,
            CreatedAt = DateTime.UtcNow
        }; 
        try
        {
            await _userRepo.AddAsync(user);
            return new ServiceResult { Success = true, Message = "User created successfully" };
        }
        catch (DbUpdateException)
        {
            return new ServiceResult { Success = false, Message = "Error creating user" };
        }
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
