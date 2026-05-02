using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class BudgetPeriodRepository : Repository<BudgetPeriod>, IBudgetPeriodRepository
{
    public BudgetPeriodRepository(BudgetingDbContext context) : base(context) { }

    public async Task<IEnumerable<BudgetPeriod>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default)
        => await _dbSet.Where(p => p.VersionId == versionId).ToListAsync(cancellationToken);

    public async Task<BudgetPeriod?> GetByVersionAndPeriodAsync(Guid versionId, string period, CancellationToken cancellationToken = default)
    {
        // period is YYYY-MM; parse year and month
        if (!DateTime.TryParseExact(period + "-01", "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None, out var dt))
            return null;

        return await _dbSet.FirstOrDefaultAsync(
            p => p.VersionId == versionId && p.Year == dt.Year && p.Month == dt.Month,
            cancellationToken);
    }
}
