namespace SistemaCore.Api.Shared.Auditing;

/// <summary>
/// Log de auditoría append-only. Lo implementa el módulo Auditoria; los demás módulos solo dependen de esta
/// interfaz y registran cada cambio de clientes y movimientos y cada evento de inicio de sesión.
/// </summary>
public interface IAuditoria
{
    Task RegistrarAsync(EventoAuditoria evento, CancellationToken ct);
}

/// <summary>Un registro: qué entidad cambió, qué acción, valores anteriores/nuevos, quién y cuándo.</summary>
public sealed record EventoAuditoria(
    string Entidad,
    string EntidadId,
    string Accion,
    IReadOnlyDictionary<string, object?>? ValoresAnteriores,
    IReadOnlyDictionary<string, object?>? ValoresNuevos,
    string Usuario,
    DateTimeOffset Fecha);
