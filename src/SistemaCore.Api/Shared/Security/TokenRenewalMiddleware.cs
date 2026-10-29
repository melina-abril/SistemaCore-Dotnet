using System.IdentityModel.Tokens.Jwt;

namespace SistemaCore.Api.Shared.Security;

public sealed class TokenRenewalMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, JwtTokenService tokens)
    {
        context.Response.OnStarting(() =>
        {
            string? jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (context.User.Identity?.IsAuthenticated == true
                && context.Response.StatusCode >= 200
                && context.Response.StatusCode < 300
                && jti is not null
                && !tokens.EstaRevocado(jti)
                && !context.Response.Headers.ContainsKey("X-Auth-Token"))
            {
                context.Response.Headers["X-Auth-Token"] = tokens.RenovarToken(context.User).Token;
            }

            return Task.CompletedTask;
        });

        await next(context);
    }
}
