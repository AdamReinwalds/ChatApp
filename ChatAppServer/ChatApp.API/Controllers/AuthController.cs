 using ChatApp.Business.Interfaces;
using ChatApp.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService, ILogger<AuthController> logger) : Controller
    {
        private readonly IAuthService _authService = authService;
        private readonly ILogger<AuthController> _logger = logger;

        public record LoginRequest(string Username, string Password);
        public record RegisterRequest(string Username, string Password);


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))            
                return BadRequest("Username and password are required");           
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);
                if (token != null)
                {
                    _logger.LogInformation("User {Username} successfully logged in", request.Username);
                    return Ok(new { token });
                }
                else
                {
                    _logger.LogWarning("Login failed for username {Username}", request.Username);
                    return Unauthorized("Invalid username or password");
                }
            }
            catch (UnauthorizedAccessException)
            {
                _logger.LogWarning("Unauthorized login attempt for username: {Username}", request.Username);
                return Unauthorized("Invalid username or password");
            }            
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Username and password are required");
            try
            {
                var response = await _authService.RegisterAsync(request.Username, request.Password);
                if (response.Success)
                {
                    _logger.LogInformation("User {Username} successfully registered", request.Username);
                    return Ok(response);
                }
                
                _logger.LogWarning("Registration failed for username {Username}: {Message}", request.Username, response.Message);
                return Conflict(response);
                
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation during registration for {Username}", request.Username);
                return Conflict("Registration failed. Username may already exist.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during registration for {Username}", request.Username);
                throw;
            }
        }
    }
}
