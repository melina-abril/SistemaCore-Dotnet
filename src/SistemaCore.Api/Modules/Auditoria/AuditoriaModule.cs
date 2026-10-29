using SistemaCore.Api.Modules.Auditoria.Application;
using SistemaCore.Api.Modules.Auditoria.Domain;
using SistemaCore.Api.Modules.Auditoria.Infrastructure;
using SistemaCore.Api.Shared.Auditing;

namespace SistemaCore.Api.Modules.Auditoria;

public static class AuditoriaModule
{
    public static IServiceCollection AddAuditoriaModule(this IServiceCollection services)
    {
        services.AddSingleton<IAuditoriaRepository, AuditoriaRepositoryEnMemoria>();
        services.AddScoped<AuditoriaService>();
        services.AddScoped<IAuditoria>(provider => provider.GetRequiredService<AuditoriaService>());
        services.AddScoped<IAuditoriaConsulta>(provider => provider.GetRequiredService<AuditoriaService>());
        return services;
    }
}
