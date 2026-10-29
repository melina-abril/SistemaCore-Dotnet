using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCore.Api.Modules.Inicio.Application;

namespace SistemaCore.Api.Modules.Inicio.Api;

[ApiController]
[Route("api/inicio")]
[Authorize]
public sealed class InicioController(IResumenDiaService service) : ControllerBase
{
    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenDiaResponse>> ObtenerResumen(CancellationToken ct) =>
        Ok(await service.ObtenerAsync(ct));
}
