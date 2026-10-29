using System.Collections.Concurrent;
using SistemaCore.Api.Modules.Autenticacion.Domain;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Autenticacion.Infrastructure;

/// <summary>Adaptador de ejemplo sin base aprobada; se reemplaza al conectar la persistencia.</summary>
public sealed class UsuarioRepositoryEnMemoria : IUsuarioRepository
{
    private readonly ConcurrentDictionary<string, Usuario> usuarios = new(StringComparer.OrdinalIgnoreCase);

    public UsuarioRepositoryEnMemoria()
    {
        Agregar(new Usuario("usr-1", "operador", "Operador de ejemplo", BCrypt.Net.BCrypt.HashPassword("Operador123!"), Roles.Operador));
        Agregar(new Usuario("usr-2", "supervisor", "Supervisor de ejemplo", BCrypt.Net.BCrypt.HashPassword("Supervisor123!"), Roles.Supervisor));
    }

    public Task<Usuario?> BuscarPorNombreAsync(string nombreUsuario, CancellationToken ct)
    {
        usuarios.TryGetValue(nombreUsuario, out Usuario? usuario);
        return Task.FromResult(usuario);
    }

    public Task<Usuario?> ObtenerAsync(string id, CancellationToken ct) =>
        Task.FromResult(usuarios.Values.FirstOrDefault(usuario => usuario.Id == id));

    public Task GuardarAsync(Usuario usuario, CancellationToken ct)
    {
        usuarios[usuario.NombreUsuario] = usuario;
        return Task.CompletedTask;
    }

    private void Agregar(Usuario usuario) => usuarios[usuario.NombreUsuario] = usuario;
}
