using System.Security.Claims;
using SistemaCore.Api.Modules.Autenticacion.Domain;
using SistemaCore.Api.Shared.Auditing;
using SistemaCore.Api.Shared.Errors;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Autenticacion.Application;

public interface ILoginService
{
    Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct);
    Task LogoutAsync(ClaimsPrincipal principal, CancellationToken ct);
}

public sealed class LoginService(
    IUsuarioRepository repository,
    IAuditoria auditoria,
    JwtTokenService tokens,
    ICurrentUser currentUser,
    TimeProvider clock) : ILoginService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Usuario) || string.IsNullOrWhiteSpace(request.Contrasena))
        {
            throw new BusinessException("AUTH-01", "Ingresa el usuario y la contraseña.");
        }

        string nombreUsuario = request.Usuario.Trim();
        Usuario? usuario = await repository.BuscarPorNombreAsync(nombreUsuario, ct);
        if (usuario is null)
        {
            await AuditarAsync(nombreUsuario, "INTENTO_FALLIDO", null, ct);
            throw new CredencialesInvalidasException();
        }

        if (usuario.Estado == EstadoUsuario.BLOQUEADO)
        {
            await AuditarAsync(nombreUsuario, "ACCESO_BLOQUEADO", usuario.IntentosFallidos, ct);
            throw new UsuarioBloqueadoException();
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Contrasena, usuario.HashContrasena))
        {
            usuario.RegistrarIntentoFallido();
            await repository.GuardarAsync(usuario, ct);
            await AuditarAsync(nombreUsuario, "INTENTO_FALLIDO", usuario.IntentosFallidos, ct);
            if (usuario.Estado == EstadoUsuario.BLOQUEADO)
            {
                await AuditarAsync(nombreUsuario, "BLOQUEO", usuario.IntentosFallidos, ct);
                throw new UsuarioBloqueadoException();
            }

            throw new CredencialesInvalidasException();
        }

        usuario.RegistrarIngresoCorrecto();
        await repository.GuardarAsync(usuario, ct);
        TokenEmitido token = tokens.EmitirToken(usuario.NombreUsuario, usuario.Nombre, usuario.Rol);
        await AuditarAsync(usuario.NombreUsuario, "INGRESO", null, ct);
        return new LoginResponse(token.Token, token.ExpiraEn, usuario.Nombre, usuario.Rol);
    }

    public async Task LogoutAsync(ClaimsPrincipal principal, CancellationToken ct)
    {
        await AuditarAsync(currentUser.Usuario, "CIERRE_SESION", null, ct);
        tokens.RevocarToken(principal);
    }

    private Task AuditarAsync(string nombreUsuario, string accion, int? intentos, CancellationToken ct)
    {
        IReadOnlyDictionary<string, object?>? nuevos = intentos is null
            ? null
            : new Dictionary<string, object?> { ["intentosFallidos"] = intentos.Value };
        return auditoria.RegistrarAsync(
            new EventoAuditoria("USUARIO", nombreUsuario, accion, null, nuevos, nombreUsuario, clock.GetUtcNow()), ct);
    }
}
