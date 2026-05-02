using ITBudgeting.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ITBudgeting.Infrastructure.Data;

public class BudgetingDbContext : DbContext
{
    public BudgetingDbContext(DbContextOptions<BudgetingDbContext> options) : base(options) { }

    public DbSet<BudgetVersion> BudgetVersions => Set<BudgetVersion>();
    public DbSet<BudgetLine> BudgetLines => Set<BudgetLine>();
    public DbSet<CostCenter> CostCenters => Set<CostCenter>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<PurchaseRequest> PurchaseRequests => Set<PurchaseRequest>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<BudgetTransfer> BudgetTransfers => Set<BudgetTransfer>();
    public DbSet<BudgetAudit> BudgetAudits => Set<BudgetAudit>();
    public DbSet<BudgetPeriod> BudgetPeriods => Set<BudgetPeriod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BudgetVersion>(e =>
        {
            e.HasKey(v => v.Id);
            e.Property(v => v.Name).HasMaxLength(200).IsRequired();
            e.Property(v => v.CreatedBy).HasMaxLength(200).IsRequired();
            e.HasMany(v => v.BudgetLines).WithOne(l => l.Version).HasForeignKey(l => l.VersionId);
            e.HasMany(v => v.BudgetPeriods).WithOne(p => p.Version).HasForeignKey(p => p.VersionId);
        });

        modelBuilder.Entity<BudgetLine>(e =>
        {
            e.HasKey(l => l.Id);
            e.Property(l => l.Period).HasMaxLength(7).IsRequired(); // YYYY-MM
            e.Property(l => l.PlannedAmount).HasColumnType("decimal(18,2)");
            e.Property(l => l.ApprovedAmount).HasColumnType("decimal(18,2)");
            e.Property(l => l.CommittedAmount).HasColumnType("decimal(18,2)");
            e.Property(l => l.ActualAmount).HasColumnType("decimal(18,2)");
            e.Ignore(l => l.RemainingAmount);
            e.Ignore(l => l.IsOverBudget);
            e.HasOne(l => l.CostCenter).WithMany(c => c.BudgetLines).HasForeignKey(l => l.CostCenterId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(l => l.Project).WithMany(p => p.BudgetLines).HasForeignKey(l => l.ProjectId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CostCenter>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Name).HasMaxLength(200).IsRequired();
            e.Property(c => c.Department).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Name).HasMaxLength(200).IsRequired();
            e.HasOne(p => p.CostCenter).WithMany(c => c.Projects).HasForeignKey(p => p.CostCenterId);
        });

        modelBuilder.Entity<PurchaseRequest>(e =>
        {
            e.HasKey(pr => pr.Id);
            e.Property(pr => pr.Amount).HasColumnType("decimal(18,2)");
            e.Property(pr => pr.SharePointUrl).HasMaxLength(500);
            e.Property(pr => pr.CreatedBy).HasMaxLength(200).IsRequired();
            e.HasOne(pr => pr.PurchaseOrder).WithOne(po => po.PurchaseRequest)
                .HasForeignKey<PurchaseOrder>(po => po.PurchaseRequestId);
        });

        modelBuilder.Entity<PurchaseOrder>(e =>
        {
            e.HasKey(po => po.Id);
            e.Property(po => po.AmountApproved).HasColumnType("decimal(18,2)");
            e.Property(po => po.AmountUsed).HasColumnType("decimal(18,2)");
            e.Ignore(po => po.RemainingAmount);
        });

        modelBuilder.Entity<BudgetTransfer>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Amount).HasColumnType("decimal(18,2)");
            e.Property(t => t.Period).HasMaxLength(7).IsRequired();
            e.Property(t => t.Reason).HasMaxLength(500).IsRequired();
            e.Property(t => t.ApprovedBy).HasMaxLength(200).IsRequired();
            e.Property(t => t.CreatedBy).HasMaxLength(200).IsRequired();
            e.HasOne(t => t.FromProject).WithMany().HasForeignKey(t => t.FromProjectId).OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.ToProject).WithMany().HasForeignKey(t => t.ToProjectId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BudgetAudit>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.EntityType).HasMaxLength(100).IsRequired();
            e.Property(a => a.Field).HasMaxLength(100).IsRequired();
            e.Property(a => a.OldValue).HasMaxLength(1000);
            e.Property(a => a.NewValue).HasMaxLength(1000);
            e.Property(a => a.ChangedBy).HasMaxLength(200).IsRequired();
        });

        modelBuilder.Entity<BudgetPeriod>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => new { p.VersionId, p.Month, p.Year }).IsUnique();
        });
    }
}
