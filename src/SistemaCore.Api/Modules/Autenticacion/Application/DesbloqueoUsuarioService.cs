using SistemaCore.Api.Modules.Autenticacion.Domain;
using SistemaCore.Api.Shared.Auditing;
using SistemaCore.Api.Shared.Errors;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Autenticacion.Application;

public interface IDesbloqueoUsuarioService
{
    Task DesbloquearAsync(string id, CancellationToken ct);
}

public sealed class DesbloqueoUsuarioService(
    IUsuarioRepository repository,
    IAuditoria auditoria,
    ICurrentUser currentUser,
    TimeProvider clock) : IDesbloqueoUsuarioService
{
    public async Task DesbloquearAsync(string id, CancellationToken ct)
    {
        if (currentUser.Rol != Roles.Supervisor)
        {
            throw new BusinessException("AUTH-02", "Solo un supervisor puede desbloquear usuarios.");
        }

        Usuario usuario = await repository.ObtenerAsync(id, ct)
            ?? throw new NotFoundException("No encontramos el usuario.");
        var anteriores = new Dictionary<string, object?>
        {
            ["estado"] = usuario.Estado.ToString(),
            ["intentosFallidos"] = usuario.IntentosFallidos
        };
        usuario.Desbloquear();
        await repository.GuardarAsync(usuario, ct);
        await auditoria.RegistrarAsync(
            new EventoAuditoria(
                "USUARIO", usuario.Id, "DESBLOQUEO", anteriores,
                new Dictionary<string, object?>
                {
                    ["estado"] = usuario.Estado.ToString(),
                    ["intentosFallidos"] = usuario.IntentosFallidos
                },
                currentUser.Usuario, clock.GetUtcNow()), ct);
    }
}
