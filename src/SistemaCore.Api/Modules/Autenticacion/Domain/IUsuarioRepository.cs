namespace SistemaCore.Api.Modules.Autenticacion.Domain;

public interface IUsuarioRepository
{
    Task<Usuario?> BuscarPorNombreAsync(string nombreUsuario, CancellationToken ct);
    Task<Usuario?> ObtenerAsync(string id, CancellationToken ct);
    Task GuardarAsync(Usuario usuario, CancellationToken ct);
}
