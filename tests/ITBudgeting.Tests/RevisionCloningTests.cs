using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Infrastructure.Data;
using ITBudgeting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITBudgeting.Tests;

public class RevisionCloningTests
{
    private BudgetingDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BudgetingDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new BudgetingDbContext(options);
    }

    [Fact]
    public async Task CloneRevision_ShouldCopyAllBudgetLines()
    {
        var db = CreateContext(nameof(CloneRevision_ShouldCopyAllBudgetLines));

        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var project = new Project("Proj A", costCenter.Id, BudgetCategory.CapEx);
        db.Projects.Add(project);

        var parent = new BudgetVersion("2024 Approved", 2024, BudgetVersionType.Approved, "admin");
        db.BudgetVersions.Add(parent);
        // Manually approve
        parent.Submit();
        parent.Approve();

        var line1 = new BudgetLine(parent.Id, costCenter.Id, project.Id, BudgetCategory.CapEx, "2024-01", 5000m);
        var line2 = new BudgetLine(parent.Id, costCenter.Id, project.Id, BudgetCategory.CapEx, "2024-02", 3000m);
        db.BudgetLines.AddRange(line1, line2);
        await db.SaveChangesAsync();

        var sut = new BudgetVersionService(
            new BudgetVersionRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        var revision = await sut.CloneRevisionAsync(parent.Id, new CloneRevisionDto(
            "2024 Revision 1", "admin", new List<int> { 1, 2, 3 }));

        var clonedLines = db.BudgetLines.Where(l => l.VersionId == revision.Id).ToList();
        Assert.Equal(2, clonedLines.Count);
        Assert.Equal(BudgetVersionType.Revision, revision.Type);
        Assert.Equal(parent.Id, revision.ParentVersionId);
    }

    [Fact]
    public async Task CloneRevision_ShouldOnlyOpenSpecifiedMonths()
    {
        var db = CreateContext(nameof(CloneRevision_ShouldOnlyOpenSpecifiedMonths));

        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var project = new Project("Proj A", costCenter.Id, BudgetCategory.CapEx);
        db.Projects.Add(project);

        var parent = new BudgetVersion("2024 Approved", 2024, BudgetVersionType.Approved, "admin");
        parent.Submit();
        parent.Approve();
        db.BudgetVersions.Add(parent);
        await db.SaveChangesAsync();

        var sut = new BudgetVersionService(
            new BudgetVersionRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        // Open only months 4 and 5
        var revision = await sut.CloneRevisionAsync(parent.Id, new CloneRevisionDto(
            "2024 Revision", "admin", new List<int> { 4, 5 }));

        var periods = db.BudgetPeriods.Where(p => p.VersionId == revision.Id).ToList();
        Assert.Equal(12, periods.Count);

        var openPeriods = periods.Where(p => p.IsOpen).ToList();
        Assert.Equal(2, openPeriods.Count);
        Assert.Contains(openPeriods, p => p.Month == 4);
        Assert.Contains(openPeriods, p => p.Month == 5);
    }

    [Fact]
    public async Task CloneRevision_FromDraftVersion_ShouldThrow()
    {
        var db = CreateContext(nameof(CloneRevision_FromDraftVersion_ShouldThrow));

        var parent = new BudgetVersion("2024 Draft", 2024, BudgetVersionType.Draft, "admin");
        db.BudgetVersions.Add(parent);
        await db.SaveChangesAsync();

        var sut = new BudgetVersionService(
            new BudgetVersionRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        await Assert.ThrowsAsync<BudgetDomainException>(() =>
            sut.CloneRevisionAsync(parent.Id, new CloneRevisionDto("Revision", "admin", new List<int> { 1 })));
    }
}
