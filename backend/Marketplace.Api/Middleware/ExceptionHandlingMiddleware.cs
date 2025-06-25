using System.Text.Json;

namespace Marketplace.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ExceptionMeddlewareHandler(context, ex);
            }
        }

        private async Task ExceptionMeddlewareHandler(HttpContext context, Exception exception)
        {
            var statusCode = exception switch
            {
                ArgumentException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status404NotFound
            };

            var result = JsonSerializer.Serialize(new
            {
                error = exception.Message,
                type = exception.GetType().Name
            });

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result);
        }
    }
}
