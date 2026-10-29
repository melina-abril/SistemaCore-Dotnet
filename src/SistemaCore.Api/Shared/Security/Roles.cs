namespace SistemaCore.Api.Shared.Security;

/// <summary>Roles del sistema: van en el claim de rol del token y en [Authorize(Roles = Roles.Supervisor)].</summary>
public static class Roles
{
    public const string Operador = "OPERADOR";
    public const string Supervisor = "SUPERVISOR";
}
