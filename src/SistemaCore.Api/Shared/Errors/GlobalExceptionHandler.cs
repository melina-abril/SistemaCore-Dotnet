using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SistemaCore.Api.Shared.Errors;

/// <summary>
/// Traduce las excepciones a ProblemDetails. Program.cs lo registra con
/// builder.Services.AddExceptionHandler&lt;GlobalExceptionHandler&gt;() y builder.Services.AddProblemDetails(),
/// y lo activa con app.UseExceptionHandler() antes de la autenticación.
/// </summary>
public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        (int Status, string Detail, string? Codigo) respuesta = exception switch
        {
            CredencialesInvalidasException e => (StatusCodes.Status401Unauthorized, e.Message, null),
            UsuarioBloqueadoException e => (StatusCodes.Status423Locked, e.Message, null),
            NotFoundException e => (StatusCodes.Status404NotFound, e.Message, null),
            BusinessException e => (StatusCodes.Status409Conflict, e.Message, e.Codigo),
            _ => (StatusCodes.Status500InternalServerError, "Ocurrió un error inesperado. Intenta nuevamente.", null),
        };
        if (respuesta.Status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Error no controlado");
        }

        var problem = new ProblemDetails { Status = respuesta.Status, Detail = respuesta.Detail };
        if (respuesta.Codigo is not null)
        {
            problem.Extensions["codigo"] = respuesta.Codigo;
        }
        httpContext.Response.StatusCode = respuesta.Status;
        await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
        return true;
    }
}
