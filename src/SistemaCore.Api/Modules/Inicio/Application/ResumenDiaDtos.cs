namespace SistemaCore.Api.Modules.Inicio.Application;

public sealed record ResumenDiaResponse(
    string Nombre,
    string Rol,
    string FechaOperativa,
    int CantidadMovimientos,
    string EstadoCierre);
