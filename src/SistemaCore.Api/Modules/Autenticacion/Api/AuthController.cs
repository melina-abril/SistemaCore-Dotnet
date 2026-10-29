using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaCore.Api.Modules.Autenticacion.Application;
using SistemaCore.Api.Shared.Security;

namespace SistemaCore.Api.Modules.Autenticacion.Api;

[ApiController]
[Route("api/auth")]
[Authorize]
public sealed class AuthController(ILoginService loginService, IDesbloqueoUsuarioService desbloqueoService) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(LoginRequest request, CancellationToken ct) =>
        Ok(await loginService.LoginAsync(request, ct));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await loginService.LogoutAsync(User, ct);
        return NoContent();
    }

    [HttpPost("usuarios/{id}/desbloqueo")]
    [Authorize(Roles = Roles.Supervisor)]
    public async Task<IActionResult> Desbloquear(string id, CancellationToken ct)
    {
        await desbloqueoService.DesbloquearAsync(id, ct);
        return NoContent();
    }
}
