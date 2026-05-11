using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IBudgetTransferRepository : IRepository<BudgetTransfer>
{
    Task<decimal> GetTotalTransferredFromProjectAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default);
    Task<IEnumerable<BudgetTransfer>> GetByVersionAsync(Guid versionId, CancellationToken cancellationToken = default);
}
