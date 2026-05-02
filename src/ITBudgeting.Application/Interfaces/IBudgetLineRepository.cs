using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IBudgetLineRepository : IRepository<BudgetLine>
{
    Task<IEnumerable<BudgetLine>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BudgetLine>> GetByFilterAsync(Guid? versionId, Guid? costCenterId, Guid? projectId, CancellationToken cancellationToken = default);
    Task<BudgetLine?> GetByProjectAndPeriodAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default);
}
