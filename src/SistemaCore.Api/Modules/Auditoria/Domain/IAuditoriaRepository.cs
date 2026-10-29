namespace SistemaCore.Api.Modules.Auditoria.Domain;

/// <summary>Puerto append-only: no expone operaciones de modificación ni eliminación.</summary>
public interface IAuditoriaRepository
{
    Task AgregarAsync(RegistroAuditoria registro, CancellationToken ct);
    Task<IReadOnlyList<RegistroAuditoria>> ListarAsync(CancellationToken ct);
}
