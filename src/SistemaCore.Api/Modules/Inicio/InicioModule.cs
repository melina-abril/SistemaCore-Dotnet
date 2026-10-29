using Microsoft.Extensions.DependencyInjection.Extensions;
using SistemaCore.Api.Modules.Inicio.Application;
using SistemaCore.Api.Modules.Inicio.Domain;
using SistemaCore.Api.Shared.Domain;

namespace SistemaCore.Api.Modules.Inicio;

/// <summary>Registra Inicio y sus fuentes provisionales hasta incorporar Movimientos y Cierre.</summary>
public static class InicioModule
{
    public static IServiceCollection AddInicioModule(this IServiceCollection services)
    {
        services.AddScoped<IResumenDiaService, ResumenDiaService>();
        services.TryAddSingleton<IFechaOperativaProvider, FechaOperativaEnMemoria>();
        services.TryAddScoped<IResumenMovimientosPort, SinMovimientos>();
        services.TryAddScoped<IEstadoCierrePort, SinCierre>();
        return services;
    }

    private sealed class FechaOperativaEnMemoria : IFechaOperativaProvider
    {
        // Fecha operativa dummy hasta conectar la fuente de datos vigente.
        public Task<DateOnly> ObtenerVigenteAsync(CancellationToken ct) =>
            Task.FromResult(new DateOnly(2026, 11, 2));
    }

    private sealed class SinMovimientos : IResumenMovimientosPort
    {
        // Respuesta dummy que reemplazará el módulo Movimientos.
        public Task<int> ContarAsync(DateOnly fechaOperativa, CancellationToken ct) =>
            Task.FromResult(0);
    }

    private sealed class SinCierre : IEstadoCierrePort
    {
        // Respuesta dummy que reemplazará el módulo Cierre.
        public Task<string> ObtenerEstadoAsync(DateOnly fechaOperativa, CancellationToken ct) =>
            Task.FromResult("Pendiente");
    }
}
