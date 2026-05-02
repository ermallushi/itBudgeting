using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Interfaces;

public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
{
    Task<IEnumerable<PurchaseOrder>> GetByProjectAsync(Guid projectId, string period, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalApprovedAmountByProjectAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default);
}
