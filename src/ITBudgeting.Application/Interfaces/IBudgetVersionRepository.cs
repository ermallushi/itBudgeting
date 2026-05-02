using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IBudgetVersionRepository : IRepository<BudgetVersion>
{
    Task<BudgetVersion?> GetWithLinesAsync(Guid id, CancellationToken cancellationToken = default);
}
