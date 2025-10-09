namespace ChatApp.API.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.{Path}", httpContext.Request.Path);
            httpContext.Response.StatusCode = 500;
            httpContext.Response.ContentType = "application/json";
            var response = new { message = "An internal server error occurred." };
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
