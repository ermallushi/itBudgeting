using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IBudgetPeriodRepository : IRepository<BudgetPeriod>
{
    Task<IEnumerable<BudgetPeriod>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default);
    Task<BudgetPeriod?> GetByVersionAndPeriodAsync(Guid versionId, string period, CancellationToken cancellationToken = default);
}
