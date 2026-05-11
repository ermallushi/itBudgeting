using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
{
    public PurchaseOrderRepository(BudgetingDbContext context) : base(context) { }

    public async Task<IEnumerable<PurchaseOrder>> GetByProjectAsync(Guid projectId, string period, CancellationToken cancellationToken = default)
        => await _dbSet
            .Include(po => po.PurchaseRequest)
            .Where(po => po.PurchaseRequest != null && po.PurchaseRequest.ProjectId == projectId)
            .ToListAsync(cancellationToken);

    public async Task<decimal> GetTotalApprovedAmountByProjectAsync(Guid versionId, Guid projectId, string period, CancellationToken cancellationToken = default)
    {
        // Sum all approved PO amounts for a given project/period
        return await _dbSet
            .Include(po => po.PurchaseRequest)
            .Where(po => po.PurchaseRequest != null &&
                         po.PurchaseRequest.ProjectId == projectId)
            .SumAsync(po => po.AmountApproved, cancellationToken);
    }
}
