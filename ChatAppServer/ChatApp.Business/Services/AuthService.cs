using ChatApp.Business.Interfaces;
using ChatApp.Business.Results;
using ChatApp.Data.Entities;
using ChatApp.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using BCryptNet = BCrypt.Net.BCrypt;


namespace ChatApp.Business.Services;

public class AuthService(IUserRepository userRepo, TokenManager tokenManager, ILogger<AuthService> logger) : IAuthService
{
    private readonly IUserRepository _userRepo = userRepo;
    private readonly TokenManager _tokenManager = tokenManager;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<ServiceResult> RegisterAsync(string username, string password)
    {
        var existingUser = await _userRepo.ExistsAsync(u => u.Username == username);
        if (existingUser == true)
        {
            _logger.LogWarning("Registration failed: Username {Username} already exists", username);
            return new ServiceResult { Success = false, Message = "Username already exists" };
        }
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
            _logger.LogInformation("User {Username} created successfully", username);
            return new ServiceResult { Success = true, Message = "User created successfully" };
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while creating user {Username}", username);
            return new ServiceResult { Success = false, Message = "Error creating user" };
        }
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        var user = await _userRepo.GetAsync(u => u.Username == username);
        if (user == null)
        {
            _logger.LogWarning("Login failed: Username {Username} not found", username);
            throw new UnauthorizedAccessException("Invalid username or password");
        }
        if (!VerifyPassword(password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: Invalid password for username {Username}", username);
            throw new UnauthorizedAccessException("Invalid password");
        }
        var token = _tokenManager.GenerateJwtToken(user);
        _logger.LogInformation("User {Username} logged in successfully", username);
        return token;
    }

     
    private bool VerifyPassword(string password, string hash) => BCryptNet.Verify(password, hash);
    private string HashPassword(string password) => BCryptNet.HashPassword(password);
}
