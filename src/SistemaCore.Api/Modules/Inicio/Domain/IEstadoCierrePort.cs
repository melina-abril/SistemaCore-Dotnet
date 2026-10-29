namespace SistemaCore.Api.Modules.Inicio.Domain;

/// <summary>Puerto hacia Cierre para consultar el estado de una fecha operativa.</summary>
public interface IEstadoCierrePort
{
    Task<string> ObtenerEstadoAsync(DateOnly fechaOperativa, CancellationToken ct);
}
