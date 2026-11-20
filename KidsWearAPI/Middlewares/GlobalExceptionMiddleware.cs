
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
                _logger.LogError(
                    ex,
                    "An unexpected error occurred while processing {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path
                );

                if (context.Response.HasStarted)
                {
                    throw;
                }

                context.Response.Clear();
                context.Response.ContentType = "application/json";

                var statusCode = ex is ArgumentException
                    ? StatusCodes.Status400BadRequest
                    : StatusCodes.Status500InternalServerError;

                context.Response.StatusCode = statusCode;

                var response = new
                {
                    status = statusCode,
                    message = statusCode == 400
                        ? "Invalid request."
                        : "An unexpected error occurred. Please try again later."
                };

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
