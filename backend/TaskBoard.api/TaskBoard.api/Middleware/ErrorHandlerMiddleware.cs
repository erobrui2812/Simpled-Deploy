using System.Text.Json;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception error)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = error switch
            {
                KeyNotFoundException => 404,
                UnauthorizedAccessException => 403,
                _ => 500
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                Message = error.Message,
                StackTrace = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                    ? error.StackTrace : null
            }));
        }
    }
}


