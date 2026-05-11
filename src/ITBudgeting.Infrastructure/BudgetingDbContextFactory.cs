using ITBudgeting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ITBudgeting.Infrastructure;

public class BudgetingDbContextFactory : IDesignTimeDbContextFactory<BudgetingDbContext>
{
    public BudgetingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<BudgetingDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=ITBudgetingDb;Trusted_Connection=True;TrustServerCertificate=True");
        return new BudgetingDbContext(optionsBuilder.Options);
    }
}
