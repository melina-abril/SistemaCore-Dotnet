# Sistema Core · API

Backend ASP.NET Core 8 del alcance actual: inicio de sesión, pantalla de inicio y auditoría de acceso. Los módulos se registran desde `Program.cs` mediante sus métodos `Add...Module()`.

## Arranque

Requiere el SDK de .NET 8. Desde la raíz del repositorio:

```sh
dotnet run --project src/SistemaCore.Api
dotnet test
```

El perfil de desarrollo escucha en `http://localhost:5080`. Swagger está disponible en `/swagger` únicamente en Development. Para compilar sin arrancar, ejecuta `dotnet build`.

## Usuarios de ejemplo

Sin base de datos, el repositorio en memoria incluye `operador` / `Operador123!` (OPERADOR) y `supervisor` / `Supervisor123!` (SUPERVISOR). Estos usuarios son solo para desarrollo. La fecha operativa inicial sin base es 2026-11-02.

## Configuración y persistencia

`appsettings.Development.json` contiene un secreto JWT **de ejemplo**, no apto para producción. Fuera de Development, configura `Jwt__Secret` con un valor privado de al menos 32 bytes; los demás parámetros JWT están en `appsettings.json`. Los tokens vencen después de 30 minutos sin actividad y el cierre de sesión los revoca en memoria.

Esta generación no tiene una base aprobada: los repositorios usan memoria y se reinician con el proceso. Para pasar a PostgreSQL cuando se apruebe su esquema, agrega la referencia central `Npgsql.EntityFrameworkCore.PostgreSQL` al proyecto Web, implementa los repositorios EF y registra `SistemaCoreDbContext` con `ConnectionStrings__Default`. La base debe crear sus tablas mediante sus propias migraciones; la API no debe ejecutar `EnsureCreated` ni `Database.Migrate`. Configura la cadena de conexión y el secreto mediante variables de entorno en el despliegue.
