using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Infrastructure.Data;
using ITBudgeting.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ITBudgeting.Tests;

public class PeriodLockTests
{
    private BudgetingDbContext CreateContext(string dbName)
    {
        var options = new DbContextOptionsBuilder<BudgetingDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
        return new BudgetingDbContext(options);
    }

    [Fact]
    public async Task CreateBudgetLine_WhenPeriodIsFullLocked_ShouldThrow()
    {
        var db = CreateContext(nameof(CreateBudgetLine_WhenPeriodIsFullLocked_ShouldThrow));

        // Seed a draft version and locked period
        var version = new BudgetVersion("Test Version", 2024, BudgetVersionType.Draft, "testuser");
        db.BudgetVersions.Add(version);
        var period = new BudgetPeriod(version.Id, 3, 2024, isOpen: false, PeriodLockType.FullLock);
        db.BudgetPeriods.Add(period);
        await db.SaveChangesAsync();

        var sut = new BudgetLineService(
            new BudgetLineRepository(db),
            new BudgetVersionRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        var costCenterId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        await Assert.ThrowsAsync<BudgetDomainException>(() =>
            sut.CreateAsync(new CreateBudgetLineDto(
                version.Id, costCenterId, projectId,
                BudgetCategory.OpEx, "2024-03", 1000m)));
    }

    [Fact]
    public async Task CreateBudgetLine_WhenPeriodIsOpen_ShouldSucceed()
    {
        var db = CreateContext(nameof(CreateBudgetLine_WhenPeriodIsOpen_ShouldSucceed));

        var version = new BudgetVersion("Test Version", 2024, BudgetVersionType.Draft, "testuser");
        db.BudgetVersions.Add(version);
        var period = new BudgetPeriod(version.Id, 3, 2024, isOpen: true, PeriodLockType.None);
        db.BudgetPeriods.Add(period);
        var costCenter = new CostCenter("IT", "Tech");
        db.CostCenters.Add(costCenter);
        var project = new Project("Project A", costCenter.Id, BudgetCategory.OpEx);
        db.Projects.Add(project);
        await db.SaveChangesAsync();

        var sut = new BudgetLineService(
            new BudgetLineRepository(db),
            new BudgetVersionRepository(db),
            new BudgetPeriodRepository(db),
            new UnitOfWork(db));

        var result = await sut.CreateAsync(new CreateBudgetLineDto(
            version.Id, costCenter.Id, project.Id,
            BudgetCategory.OpEx, "2024-03", 1000m));

        Assert.NotNull(result);
        Assert.Equal(1000m, result.PlannedAmount);
    }

    [Fact]
    public void BudgetPeriod_IsLocked_WhenIsOpenFalse()
    {
        var period = new BudgetPeriod(Guid.NewGuid(), 1, 2024, isOpen: false, PeriodLockType.FullLock);
        Assert.True(period.IsLocked());
    }

    [Fact]
    public void BudgetPeriod_IsNotLocked_WhenIsOpenTrue()
    {
        var period = new BudgetPeriod(Guid.NewGuid(), 1, 2024, isOpen: true, PeriodLockType.None);
        Assert.False(period.IsLocked());
    }
}
