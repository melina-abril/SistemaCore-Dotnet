using SistemaCore.Api.Modules.Autenticacion.Application;
using SistemaCore.Api.Modules.Autenticacion.Domain;
using SistemaCore.Api.Modules.Autenticacion.Infrastructure;

namespace SistemaCore.Api.Modules.Autenticacion;

public static class AutenticacionModule
{
    public static IServiceCollection AddAutenticacionModule(this IServiceCollection services)
    {
        services.AddSingleton<IUsuarioRepository, UsuarioRepositoryEnMemoria>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IDesbloqueoUsuarioService, DesbloqueoUsuarioService>();
        return services;
    }
}
