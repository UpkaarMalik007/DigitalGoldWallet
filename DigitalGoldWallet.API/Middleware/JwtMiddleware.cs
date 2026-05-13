using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DigitalGoldWallet.API.Middlewares;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;

        if (user.Identity != null &&
            user.Identity.IsAuthenticated)
        {
            var userId =
                user.FindFirst(ClaimTypes.NameIdentifier)
                    ?.Value;

            context.Items["UserId"] = userId;
        }

        await _next(context);
    }
}