using ITBudgeting.Application.Interfaces;
using ITBudgeting.Infrastructure.Data;

namespace ITBudgeting.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BudgetingDbContext _context;

    public UnitOfWork(BudgetingDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
