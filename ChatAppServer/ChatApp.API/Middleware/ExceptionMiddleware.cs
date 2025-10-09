namespace ChatApp.API.Middleware;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.{Path}", httpContext.Request.Path);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = 500;
            var response = new { message = "An internal server error occurred." };
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
