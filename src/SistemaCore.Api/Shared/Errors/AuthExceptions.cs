namespace SistemaCore.Api.Shared.Errors;

/// <summary>Usuario inexistente o contraseña incorrecta: 401 con el mismo mensaje en los dos casos.</summary>
public class CredencialesInvalidasException() : Exception("Usuario o contraseña inválidos");

/// <summary>Usuario BLOQUEADO por intentos fallidos: 423 aunque la contraseña sea correcta.</summary>
public class UsuarioBloqueadoException() : Exception("El usuario está bloqueado");
