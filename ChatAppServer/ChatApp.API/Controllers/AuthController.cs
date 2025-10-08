 using ChatApp.Business.Interfaces;
using ChatApp.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(IAuthService authService) : Controller
    {
        //private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService = authService;

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
                return Ok(new { token });
            }
            catch (UnauthorizedAccessException)
            {
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
                    return Ok(response);
                else
                    return Conflict(response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
