using SharedKernel.Exceptions;
using System.Text.Json;

namespace Marketplace.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await ExceptionMiddlewareHandler(context, ex);
            }
        }

        private async Task ExceptionMiddlewareHandler(HttpContext context, Exception exception)
        {
            int statusCode;
            string message = exception.Message;
            string type = exception.GetType().Name;

            if (exception is BaseException baseException)
            {
                statusCode = (int)baseException.StatusCode;
            }
            else
            {
                statusCode = StatusCodes.Status500InternalServerError;
                message = "An unexpected error occurred.";
            }

            var result = JsonSerializer.Serialize(new
            {
                error = message,
                type = type
            });

            _logger.LogError(exception, "An error occurred: {Message}", message);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(result);
        }

    }
}
