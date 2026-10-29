using SistemaCore.Api.Modules.Auditoria.Domain;

namespace SistemaCore.Api.Modules.Auditoria.Application;

public sealed record RegistroAuditoriaResponse(
    string Id,
    string Entidad,
    string EntidadId,
    string Accion,
    IReadOnlyDictionary<string, object?>? ValoresAnteriores,
    IReadOnlyDictionary<string, object?>? ValoresNuevos,
    string Usuario,
    DateTimeOffset Fecha)
{
    public static RegistroAuditoriaResponse Desde(RegistroAuditoria registro) =>
        new(registro.Id, registro.Entidad, registro.EntidadId, registro.Accion,
            registro.ValoresAnteriores, registro.ValoresNuevos, registro.Usuario, registro.Fecha);
}
