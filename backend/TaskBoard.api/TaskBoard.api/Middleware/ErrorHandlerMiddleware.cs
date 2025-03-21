using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace TaskBoard.api.Middleware
{
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
                var response = context.Response;
                response.ContentType = "application/json";

                response.StatusCode = error switch
                {
                    KeyNotFoundException _ => StatusCodes.Status404NotFound,
                    UnauthorizedAccessException _ => StatusCodes.Status401Unauthorized,
                    _ => StatusCodes.Status500InternalServerError
                };

                await response.WriteAsync(JsonSerializer.Serialize(new
                {
                    Message = error.Message,
                    StackTrace = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                        ? error.StackTrace
                        : null
                }));
            }
        }
    }
}
