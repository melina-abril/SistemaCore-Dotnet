using System.Collections.Concurrent;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace SistemaCore.Api.Shared.Security;

public sealed record TokenEmitido(string Token, DateTimeOffset ExpiraEn);

public sealed class JwtTokenService(IOptions<JwtOptions> options, TimeProvider clock)
{
    private readonly ConcurrentDictionary<string, DateTimeOffset> _revocados = new();

    public TokenEmitido EmitirToken(string usuario, string nombre, string rol)
    {
        JwtOptions config = options.Value;
        if (Encoding.UTF8.GetByteCount(config.Secret) < 32 || config.ExpiraMinutos <= 0)
        {
            throw new InvalidOperationException("La configuración JWT debe incluir un secreto de al menos 32 bytes y una duración positiva.");
        }

        DateTimeOffset ahora = clock.GetUtcNow();
        DateTimeOffset expiraEn = ahora.AddMinutes(config.ExpiraMinutos);
        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario),
                new Claim("name", nombre),
                new Claim("role", rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            }),
            Issuer = config.Issuer,
            Audience = config.Audience,
            IssuedAt = ahora.UtcDateTime,
            NotBefore = ahora.UtcDateTime,
            Expires = expiraEn.UtcDateTime,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.Secret)),
                SecurityAlgorithms.HmacSha256)
        };

        return new TokenEmitido(new JsonWebTokenHandler().CreateToken(descriptor), expiraEn);
    }

    public TokenEmitido RenovarToken(ClaimsPrincipal usuario)
    {
        string? identificador = usuario.FindFirstValue(JwtRegisteredClaimNames.Sub);
        string? nombre = usuario.FindFirstValue("name");
        string? rol = usuario.FindFirstValue("role");
        if (identificador is null || nombre is null || rol is null)
        {
            throw new InvalidOperationException("El token no contiene los datos necesarios para renovarlo.");
        }

        return EmitirToken(identificador, nombre, rol);
    }

    public void RevocarToken(ClaimsPrincipal usuario)
    {
        string? jti = usuario.FindFirstValue(JwtRegisteredClaimNames.Jti);
        string? exp = usuario.FindFirstValue(JwtRegisteredClaimNames.Exp);
        if (jti is null || !long.TryParse(exp, out long segundos))
        {
            throw new InvalidOperationException("El token no contiene los datos necesarios para revocarlo.");
        }

        RevocarToken(jti, DateTimeOffset.FromUnixTimeSeconds(segundos));
    }

    public void RevocarToken(string jti, DateTimeOffset expiraEn)
    {
        if (expiraEn <= clock.GetUtcNow())
        {
            return;
        }

        _revocados[jti] = expiraEn;
        foreach (KeyValuePair<string, DateTimeOffset> entrada in _revocados)
        {
            if (entrada.Value <= clock.GetUtcNow())
            {
                _revocados.TryRemove(entrada.Key, out _);
            }
        }
    }

    public bool EstaRevocado(string jti)
    {
        if (!_revocados.TryGetValue(jti, out DateTimeOffset expiraEn))
        {
            return false;
        }

        if (expiraEn > clock.GetUtcNow())
        {
            return true;
        }

        _revocados.TryRemove(jti, out _);
        return false;
    }
}
