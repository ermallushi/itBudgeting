using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class BudgetLineRepository : Repository<BudgetLine>, IBudgetLineRepository
{
    public BudgetLineRepository(BudgetingDbContext context) : base(context) { }

    public async Task<IEnumerable<BudgetLine>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(l => l.VersionId == versionId).ToListAsync(cancellationToken);

    public async Task<IEnumerable<BudgetLine>> GetByFilterAsync(
        Guid? versionId, Guid? costCenterId, Guid? projectId, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.AsQueryable();
        if (versionId.HasValue) query = query.Where(l => l.VersionId == versionId.Value);
        if (costCenterId.HasValue) query = query.Where(l => l.CostCenterId == costCenterId.Value);
        if (projectId.HasValue) query = query.Where(l => l.ProjectId == projectId.Value);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<BudgetLine?> GetByProjectAndPeriodAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(
            l => l.VersionId == versionId && l.ProjectId == projectId && l.Period == period,
            cancellationToken);
}
