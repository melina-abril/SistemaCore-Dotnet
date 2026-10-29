namespace SistemaCore.Api.Shared.Errors;

/// <summary>Regla de negocio incumplida (ej. BR-01, BR-02). GlobalExceptionHandler la traduce a 409 con su código.</summary>
public class BusinessException(string codigo, string mensaje) : Exception(mensaje)
{
    public string Codigo { get; } = codigo;
}
