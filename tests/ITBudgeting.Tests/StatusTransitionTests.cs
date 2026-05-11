using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;
using Xunit;

namespace ITBudgeting.Tests;

public class StatusTransitionTests
{
    [Fact]
    public void BudgetVersion_Submit_FromDraft_ShouldSucceed()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        version.Submit();
        Assert.Equal(BudgetVersionStatus.Submitted, version.Status);
    }

    [Fact]
    public void BudgetVersion_Approve_FromSubmitted_ShouldSucceed()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        version.Submit();
        version.Approve();
        Assert.Equal(BudgetVersionStatus.Approved, version.Status);
    }

    [Fact]
    public void BudgetVersion_Lock_FromApproved_ShouldSucceed()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        version.Submit();
        version.Approve();
        version.Lock();
        Assert.Equal(BudgetVersionStatus.Locked, version.Status);
    }

    [Fact]
    public void BudgetVersion_Submit_FromApproved_ShouldThrow()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        version.Submit();
        version.Approve();
        Assert.Throws<BudgetDomainException>(() => version.Submit());
    }

    [Fact]
    public void BudgetVersion_Approve_FromDraft_ShouldThrow()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        Assert.Throws<BudgetDomainException>(() => version.Approve());
    }

    [Fact]
    public void BudgetVersion_Lock_FromDraft_ShouldThrow()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        Assert.Throws<BudgetDomainException>(() => version.Lock());
    }

    [Fact]
    public void BudgetVersion_Lock_FromSubmitted_ShouldThrow()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        version.Submit();
        Assert.Throws<BudgetDomainException>(() => version.Lock());
    }

    [Fact]
    public void BudgetVersion_IsEditable_OnlyWhenDraft()
    {
        var version = new BudgetVersion("Test", 2024, BudgetVersionType.Draft, "user");
        Assert.True(version.IsEditable());
        version.Submit();
        Assert.False(version.IsEditable());
    }

    [Fact]
    public void PurchaseRequest_InvalidTransitions_ShouldThrow()
    {
        var pr = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), 1000m, "user");

        // Cannot approve from Draft (must be Submitted first)
        Assert.Throws<BudgetDomainException>(() => pr.Approve());
    }

    [Fact]
    public void PurchaseRequest_Approve_FromSubmitted_ShouldSucceed()
    {
        var pr = new PurchaseRequest(Guid.NewGuid(), Guid.NewGuid(), 1000m, "user");
        pr.Submit();
        pr.Approve();
        Assert.Equal(PurchaseRequestStatus.Approved, pr.Status);
    }
}
