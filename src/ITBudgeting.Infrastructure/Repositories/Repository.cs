using ITBudgeting.Application.Interfaces;
using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly BudgetingDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(BudgetingDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(new object[] { id }, cancellationToken);

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.ToListAsync(cancellationToken);

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await _dbSet.AddAsync(entity, cancellationToken);

    public void Update(T entity)
        => _dbSet.Update(entity);

    public void Remove(T entity)
        => _dbSet.Remove(entity);
}
