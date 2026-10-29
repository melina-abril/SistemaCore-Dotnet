using SistemaCore.Api.Modules.Auditoria.Domain;
using SistemaCore.Api.Shared.Auditing;

namespace SistemaCore.Api.Modules.Auditoria.Application;

public interface IAuditoriaConsulta
{
    Task<IReadOnlyList<RegistroAuditoriaResponse>> ListarAsync(CancellationToken ct);
}

/// <summary>Implementa el puerto compartido de escritura y la consulta de solo lectura.</summary>
public sealed class AuditoriaService(IAuditoriaRepository repository) : IAuditoria, IAuditoriaConsulta
{
    public Task RegistrarAsync(EventoAuditoria evento, CancellationToken ct)
    {
        var registro = new RegistroAuditoria(
            Guid.NewGuid().ToString(),
            evento.Entidad,
            evento.EntidadId,
            evento.Accion,
            Copiar(evento.ValoresAnteriores),
            Copiar(evento.ValoresNuevos),
            evento.Usuario,
            evento.Fecha);
        return repository.AgregarAsync(registro, ct);
    }

    public async Task<IReadOnlyList<RegistroAuditoriaResponse>> ListarAsync(CancellationToken ct)
    {
        var registros = await repository.ListarAsync(ct);
        return registros.Select(RegistroAuditoriaResponse.Desde).ToList();
    }

    private static IReadOnlyDictionary<string, object?>? Copiar(IReadOnlyDictionary<string, object?>? valores) =>
        valores is null ? null : new System.Collections.ObjectModel.ReadOnlyDictionary<string, object?>(
            new Dictionary<string, object?>(valores));
}
