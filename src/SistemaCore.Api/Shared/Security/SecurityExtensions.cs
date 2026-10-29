using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SistemaCore.Api.Shared.Security;

public static class SecurityExtensions
{
    public static IServiceCollection AddJwtSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection("Jwt"));
        services.AddSingleton<JwtTokenService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                JwtOptions config = configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
                if (Encoding.UTF8.GetByteCount(config.Secret) < 32)
                {
                    throw new InvalidOperationException("Configura Jwt:Secret con al menos 32 bytes.");
                }

                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = JwtRegisteredClaimNames.Sub,
                    RoleClaimType = "role"
                };
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        string? jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                        if (jti is null || context.HttpContext.RequestServices
                                .GetRequiredService<JwtTokenService>().EstaRevocado(jti))
                        {
                            context.Fail("La sesión ya no es válida.");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
        return services;
    }
}
