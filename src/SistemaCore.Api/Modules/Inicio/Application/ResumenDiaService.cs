using System.Globalization;
using SistemaCore.Api.Modules.Inicio.Domain;
using SistemaCore.Api.Shared.Domain;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Inicio.Application;

public interface IResumenDiaService
{
    Task<ResumenDiaResponse> ObtenerAsync(CancellationToken ct);
}

/// <summary>Consulta las fuentes vigentes en cada solicitud; no conserva un resumen en caché.</summary>
public sealed class ResumenDiaService(
    ICurrentUser currentUser,
    IFechaOperativaProvider fechaOperativa,
    IResumenMovimientosPort movimientos,
    IEstadoCierrePort cierre) : IResumenDiaService
{
    public async Task<ResumenDiaResponse> ObtenerAsync(CancellationToken ct)
    {
        DateOnly fecha = await fechaOperativa.ObtenerVigenteAsync(ct);
        int cantidad = await movimientos.ContarAsync(fecha, ct);
        string estado = await cierre.ObtenerEstadoAsync(fecha, ct);

        return new ResumenDiaResponse(
            currentUser.Nombre,
            currentUser.Rol,
            fecha.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            cantidad,
            estado);
    }
}
