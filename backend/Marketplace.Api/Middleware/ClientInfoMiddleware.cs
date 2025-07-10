namespace Marketplace.Api.Middleware
{
    public class ClientInfoMiddleware
    {
        private readonly RequestDelegate _next;

        public ClientInfoMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Items["ClientIp"] = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            context.Items["ClientDevice"] = context.Request.Headers["User-Agent"].ToString() ?? "unknown";

            await _next(context);
        }
    }

}
