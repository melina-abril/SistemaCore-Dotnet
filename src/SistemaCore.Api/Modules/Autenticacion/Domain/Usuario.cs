using SistemaCore.Api.Shared.Errors;

namespace SistemaCore.Api.Modules.Autenticacion.Domain;

public enum EstadoUsuario
{
    ACTIVO,
    BLOQUEADO
}

public sealed class Usuario
{
    public string Id { get; }
    public string NombreUsuario { get; }
    public string Nombre { get; }
    public string HashContrasena { get; }
    public string Rol { get; }
    public int IntentosFallidos { get; private set; }
    public EstadoUsuario Estado { get; private set; }

    public Usuario(string id, string nombreUsuario, string nombre, string hashContrasena, string rol, int intentosFallidos = 0, EstadoUsuario estado = EstadoUsuario.ACTIVO)
    {
        Id = id;
        NombreUsuario = nombreUsuario;
        Nombre = nombre;
        HashContrasena = hashContrasena;
        Rol = rol;
        IntentosFallidos = intentosFallidos;
        Estado = estado;
    }

    public void RegistrarIntentoFallido()
    {
        if (Estado == EstadoUsuario.BLOQUEADO)
        {
            throw new UsuarioBloqueadoException();
        }

        IntentosFallidos++;
        if (IntentosFallidos >= 5)
        {
            Estado = EstadoUsuario.BLOQUEADO;
        }
    }

    public void RegistrarIngresoCorrecto()
    {
        if (Estado == EstadoUsuario.BLOQUEADO)
        {
            throw new UsuarioBloqueadoException();
        }

        IntentosFallidos = 0;
    }

    public void Desbloquear()
    {
        IntentosFallidos = 0;
        Estado = EstadoUsuario.ACTIVO;
    }
}
