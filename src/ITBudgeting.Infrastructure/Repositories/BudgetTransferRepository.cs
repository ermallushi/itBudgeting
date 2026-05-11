using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class BudgetTransferRepository : Repository<BudgetTransfer>, IBudgetTransferRepository
{
    public BudgetTransferRepository(BudgetingDbContext context) : base(context) { }

    public async Task<decimal> GetTotalTransferredFromProjectAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default)
        => await _dbSet
            .Where(t => t.FromProjectId == projectId && t.Period == period)
            .SumAsync(t => t.Amount, cancellationToken);

    public async Task<IEnumerable<BudgetTransfer>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);
}
