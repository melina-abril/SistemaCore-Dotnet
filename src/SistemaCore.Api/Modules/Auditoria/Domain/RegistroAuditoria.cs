namespace SistemaCore.Api.Modules.Auditoria.Domain;

/// <summary>Registro inmutable de un evento de auditoría.</summary>
public sealed record RegistroAuditoria(
    string Id,
    string Entidad,
    string EntidadId,
    string Accion,
    IReadOnlyDictionary<string, object?>? ValoresAnteriores,
    IReadOnlyDictionary<string, object?>? ValoresNuevos,
    string Usuario,
    DateTimeOffset Fecha);
