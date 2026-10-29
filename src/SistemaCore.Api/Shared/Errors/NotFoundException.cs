namespace SistemaCore.Api.Shared.Errors;

/// <summary>Recurso inexistente. GlobalExceptionHandler la traduce a 404.</summary>
public class NotFoundException(string mensaje) : Exception(mensaje);
