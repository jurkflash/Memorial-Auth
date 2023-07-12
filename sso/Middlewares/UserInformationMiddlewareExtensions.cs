using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sso.Middlewares
{
    public static class UserInformationMiddlewareExtensions
    {
        public static IApplicationBuilder UseUserInformation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UserInformationMiddleware>();
        }
    }

    public class UserInformationMiddleware
    {
        private readonly RequestDelegate _next;

        public UserInformationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var claims = context.User.Identity as ClaimsIdentity;

                var userId = claims.FindFirst("UserId")?.Value;

                if (!string.IsNullOrEmpty(userId))
                {
                    context.Items["UserId"] = userId;
                }
            }

            // Call the next delegate/middleware in the pipeline
            await _next(context);
        }
    }
}
