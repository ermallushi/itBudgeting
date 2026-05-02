using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class BudgetVersionRepository : Repository<BudgetVersion>, IBudgetVersionRepository
{
    public BudgetVersionRepository(BudgetingDbContext context) : base(context) { }

    public async Task<BudgetVersion?> GetWithLinesAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(v => v.BudgetLines)
            .Include(v => v.BudgetPeriods)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
}
