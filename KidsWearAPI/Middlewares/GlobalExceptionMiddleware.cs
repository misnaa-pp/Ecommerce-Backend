
using System.Net;

namespace KidsWearAPI.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";

                if (ex is ArgumentException)
                {
                    context.Response.StatusCode =
                        StatusCodes.Status400BadRequest;
                }
                else
                {
                    context.Response.StatusCode =
                        StatusCodes.Status500InternalServerError;
                }

                var response = new
                {
                    message = ex.Message
                };

                await context.Response.WriteAsJsonAsync(response);
            }

        }
    }
}

