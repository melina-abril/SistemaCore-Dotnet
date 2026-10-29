using System.ComponentModel.DataAnnotations;

namespace SistemaCore.Api.Modules.Autenticacion.Application;

public sealed record LoginRequest(
    [Required(ErrorMessage = "Ingresa el usuario.")] string Usuario,
    [Required(ErrorMessage = "Ingresa la contraseña.")] string Contrasena);

public sealed record LoginResponse(string Token, DateTimeOffset ExpiraEn, string Nombre, string Rol);
