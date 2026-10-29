using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCore.Api.Modules.Auditoria.Application;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Auditoria.Api;

[ApiController]
[Route("api/auditoria")]
[Authorize(Roles = Roles.Supervisor)]
public sealed class AuditoriaController(IAuditoriaConsulta service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<RegistroAuditoriaResponse>>> Listar(CancellationToken ct) =>
        Ok(await service.ListarAsync(ct));
}
