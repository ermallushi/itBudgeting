using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Services;

public class AuditService
{
    private readonly IAuditRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IAuditRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetAuditDto>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct = default)
    {
        var items = await _repo.GetByEntityAsync(entityType, entityId, ct);
        return items.Select(a => new BudgetAuditDto(a.Id, a.EntityType, a.EntityId, a.Field, a.OldValue, a.NewValue, a.ChangedBy, a.ChangedAt));
    }

    public async Task RecordAsync(string entityType, Guid entityId, string field,
        string? oldValue, string? newValue, string changedBy, CancellationToken ct = default)
    {
        var audit = new BudgetAudit(entityType, entityId, field, oldValue, newValue, changedBy);
        await _repo.AddAsync(audit, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
