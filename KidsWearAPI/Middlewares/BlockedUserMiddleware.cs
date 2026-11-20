
using KidsWearAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KidsWearAPI.Middleware
{
    public class BlockedUserMiddleware
    {
        private readonly RequestDelegate _next;

        public BlockedUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            AppDbContext dbContext)
        {
            if (context.User.Identity?.IsAuthenticated == true &&
                context.User.IsInRole("User"))
            {
                var userId = context.User.FindFirst(
                    ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userId, out int id))
                {
                    bool isBlocked = await dbContext.Users
                        .AnyAsync(u =>
                            u.UserId == id && u.IsBlocked);

                    if (isBlocked)
                    {
                        context.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        await context.Response.WriteAsJsonAsync(new
                        {
                            message = "Your account has been blocked."
                        });

                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
