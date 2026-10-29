namespace SistemaCore.Api.Shared.Security;

/// <summary>
/// Usuario autenticado de la request, tomado de los claims del token (usuario, nombre y rol). Lo implementa
/// HttpCurrentUser (scoped, con IHttpContextAccessor); los casos de uso lo usan para auditoría y el resumen.
/// </summary>
public interface ICurrentUser
{
    string Usuario { get; }
    string Nombre { get; }
    string Rol { get; }
}
