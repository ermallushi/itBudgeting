using ITBudgeting.Application.Interfaces;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Infrastructure.Data;
using ITBudgeting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ITBudgeting.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BudgetingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IBudgetVersionRepository, BudgetVersionRepository>();
        services.AddScoped<IBudgetLineRepository, BudgetLineRepository>();
        services.AddScoped<IBudgetPeriodRepository, BudgetPeriodRepository>();
        services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();
        services.AddScoped<IBudgetTransferRepository, BudgetTransferRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<IRepository<CostCenter>, Repository<CostCenter>>();
        services.AddScoped<IRepository<Project>, Repository<Project>>();
        services.AddScoped<IRepository<PurchaseRequest>, Repository<PurchaseRequest>>();
        services.AddScoped<IRepository<PurchaseOrder>, Repository<PurchaseOrder>>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application Services
        services.AddScoped<BudgetVersionService>();
        services.AddScoped<BudgetLineService>();
        services.AddScoped<CostCenterService>();
        services.AddScoped<ProjectService>();
        services.AddScoped<PurchaseRequestService>();
        services.AddScoped<PurchaseOrderService>();
        services.AddScoped<BudgetTransferService>();
        services.AddScoped<BudgetPeriodService>();
        services.AddScoped<AuditService>();
        services.AddScoped<ReportService>();

        return services;
    }
}
