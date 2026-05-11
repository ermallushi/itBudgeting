using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class AuditRepository : Repository<BudgetAudit>, IAuditRepository
{
    public AuditRepository(BudgetingDbContext context) : base(context) { }

    public async Task<IEnumerable<BudgetAudit>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(a => a.EntityType == entityType && a.EntityId == entityId)
            .OrderByDescending(a => a.ChangedAt)
            .ToListAsync(cancellationToken);
}
