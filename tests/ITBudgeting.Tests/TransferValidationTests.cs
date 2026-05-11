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

public class TransferValidationTests
{
    private BudgetingDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BudgetingDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new BudgetingDbContext(options);
    }

    [Fact]
    public async Task CreateTransfer_WhenInsufficientBudget_ShouldThrow()
    {
        var db = CreateContext(nameof(CreateTransfer_WhenInsufficientBudget_ShouldThrow));

        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var fromProject = new Project("From Project", costCenter.Id, BudgetCategory.OpEx);
        var toProject = new Project("To Project", costCenter.Id, BudgetCategory.OpEx);
        db.Projects.AddRange(fromProject, toProject);

        var version = new BudgetVersion("2024", 2024, BudgetVersionType.Approved, "admin");
        version.Submit(); version.Approve();
        db.BudgetVersions.Add(version);

        // Source line with only 1000 approved and 900 committed
        var sourceLine = new BudgetLine(version.Id, costCenter.Id, fromProject.Id, BudgetCategory.OpEx, "2024-06", 1000m);
        sourceLine.UpdateAmounts(1000m, 1000m, 0m);
        sourceLine.UpdateCommittedAmount(900m); // leaves only 100 available
        db.BudgetLines.Add(sourceLine);

        var period = new BudgetPeriod(version.Id, 6, 2024, true, PeriodLockType.None);
        db.BudgetPeriods.Add(period);
        await db.SaveChangesAsync();

        var sut = new BudgetTransferService(
            new BudgetTransferRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        // Try to transfer 500 but only 100 available
        await Assert.ThrowsAsync<BudgetDomainException>(() =>
            sut.CreateAsync(new CreateBudgetTransferDto(
                version.Id, fromProject.Id, toProject.Id,
                500m, BudgetCategory.OpEx, "2024-06",
                "Test transfer", "manager", "user")));
    }

    [Fact]
    public async Task CreateTransfer_WhenSufficientBudget_ShouldSucceed()
    {
        var db = CreateContext(nameof(CreateTransfer_WhenSufficientBudget_ShouldSucceed));

        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var fromProject = new Project("From Project", costCenter.Id, BudgetCategory.OpEx);
        var toProject = new Project("To Project", costCenter.Id, BudgetCategory.OpEx);
        db.Projects.AddRange(fromProject, toProject);

        var version = new BudgetVersion("2024", 2024, BudgetVersionType.Approved, "admin");
        version.Submit(); version.Approve();
        db.BudgetVersions.Add(version);

        var sourceLine = new BudgetLine(version.Id, costCenter.Id, fromProject.Id, BudgetCategory.OpEx, "2024-06", 5000m);
        sourceLine.UpdateAmounts(5000m, 5000m, 0m);
        sourceLine.UpdateCommittedAmount(1000m); // 4000 available
        db.BudgetLines.Add(sourceLine);

        var period = new BudgetPeriod(version.Id, 6, 2024, true, PeriodLockType.None);
        db.BudgetPeriods.Add(period);
        await db.SaveChangesAsync();

        var sut = new BudgetTransferService(
            new BudgetTransferRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        var result = await sut.CreateAsync(new CreateBudgetTransferDto(
            version.Id, fromProject.Id, toProject.Id,
            2000m, BudgetCategory.OpEx, "2024-06",
            "Test transfer", "manager", "user"));

        Assert.NotNull(result);
        Assert.Equal(2000m, result.Amount);
    }

    [Fact]
    public async Task CreateTransfer_WhenPeriodLocked_ShouldThrow()
    {
        var db = CreateContext(nameof(CreateTransfer_WhenPeriodLocked_ShouldThrow));

        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var fromProject = new Project("From", costCenter.Id, BudgetCategory.OpEx);
        var toProject = new Project("To", costCenter.Id, BudgetCategory.OpEx);
        db.Projects.AddRange(fromProject, toProject);

        var version = new BudgetVersion("2024", 2024, BudgetVersionType.Approved, "admin");
        version.Submit(); version.Approve();
        db.BudgetVersions.Add(version);

        var sourceLine = new BudgetLine(version.Id, costCenter.Id, fromProject.Id, BudgetCategory.OpEx, "2024-06", 5000m);
        sourceLine.UpdateAmounts(5000m, 5000m, 0m);
        db.BudgetLines.Add(sourceLine);

        var period = new BudgetPeriod(version.Id, 6, 2024, false, PeriodLockType.FullLock);
        db.BudgetPeriods.Add(period);
        await db.SaveChangesAsync();

        var sut = new BudgetTransferService(
            new BudgetTransferRepository(db),
            new BudgetLineRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        await Assert.ThrowsAsync<BudgetDomainException>(() =>
            sut.CreateAsync(new CreateBudgetTransferDto(
                version.Id, fromProject.Id, toProject.Id,
                500m, BudgetCategory.OpEx, "2024-06",
                "Test", "manager", "user")));
    }
}
