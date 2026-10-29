namespace SistemaCore.Api.Shared.Domain;

/// <summary>Proporciona la fecha operativa vigente, que puede diferir de la fecha del calendario.</summary>
public interface IFechaOperativaProvider
{
    Task<DateOnly> ObtenerVigenteAsync(CancellationToken ct);
}
