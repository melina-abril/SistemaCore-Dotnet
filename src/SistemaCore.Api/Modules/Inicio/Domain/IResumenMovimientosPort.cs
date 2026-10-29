namespace SistemaCore.Api.Modules.Inicio.Domain;

/// <summary>Puerto hacia Movimientos para consultar la cantidad de una fecha operativa.</summary>
public interface IResumenMovimientosPort
{
    Task<int> ContarAsync(DateOnly fechaOperativa, CancellationToken ct);
}
