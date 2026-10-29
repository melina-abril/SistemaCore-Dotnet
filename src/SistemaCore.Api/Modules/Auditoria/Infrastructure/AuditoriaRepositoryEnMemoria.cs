using System.Collections.Concurrent;
using SistemaCore.Api.Modules.Auditoria.Domain;

namespace SistemaCore.Api.Modules.Auditoria.Infrastructure;

/// <summary>Adaptador append-only en memoria; los registros de ejemplo se reemplazan al conectar la base.</summary>
public sealed class AuditoriaRepositoryEnMemoria : IAuditoriaRepository
{
    private readonly ConcurrentQueue<RegistroAuditoria> registros = new();

    public AuditoriaRepositoryEnMemoria()
    {
        registros.Enqueue(new RegistroAuditoria(
            "aud-1", "USUARIO", "operador", "INGRESO", null, null,
            "operador", new DateTimeOffset(2026, 11, 2, 9, 0, 0, TimeSpan.Zero)));
        registros.Enqueue(new RegistroAuditoria(
            "aud-2", "USUARIO", "supervisor", "INGRESO", null, null,
            "supervisor", new DateTimeOffset(2026, 11, 2, 9, 15, 0, TimeSpan.Zero)));
    }

    public Task AgregarAsync(RegistroAuditoria registro, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        registros.Enqueue(registro);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<RegistroAuditoria>> ListarAsync(CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        IReadOnlyList<RegistroAuditoria> resultado = registros
            .OrderByDescending(registro => registro.Fecha)
            .ToList();
        return Task.FromResult(resultado);
    }
}
