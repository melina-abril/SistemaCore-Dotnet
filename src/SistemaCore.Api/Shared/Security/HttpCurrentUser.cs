using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SistemaCore.Api.Shared.Security;

public sealed class HttpCurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public string Usuario => accessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty;
    public string Nombre => accessor.HttpContext?.User.FindFirstValue("name") ?? string.Empty;
    public string Rol => accessor.HttpContext?.User.FindFirstValue("role") ?? string.Empty;
}
