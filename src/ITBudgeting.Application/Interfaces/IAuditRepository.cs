using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IAuditRepository : IRepository<BudgetAudit>
{
    Task<IEnumerable<BudgetAudit>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default);
}
