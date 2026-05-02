using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;
using Xunit;

namespace ITBudgeting.Tests;

public class POConsumptionTests
{
    [Fact]
    public void PurchaseOrder_RemainingAmount_ShouldEqualApprovedMinusUsed()
    {
        var po = new PurchaseOrder(Guid.NewGuid(), 10_000m);
        po.RecordUsage(3_000m);
        Assert.Equal(7_000m, po.RemainingAmount);
        Assert.Equal(PurchaseOrderStatus.PartiallyUsed, po.Status);
    }

    [Fact]
    public void PurchaseOrder_RecordFullUsage_ShouldClose()
    {
        var po = new PurchaseOrder(Guid.NewGuid(), 5_000m);
        po.RecordUsage(5_000m);
        Assert.Equal(0m, po.RemainingAmount);
        Assert.Equal(PurchaseOrderStatus.Closed, po.Status);
    }

    [Fact]
    public void PurchaseOrder_RecordUsageExceedingApproved_ShouldThrow()
    {
        var po = new PurchaseOrder(Guid.NewGuid(), 1_000m);
        Assert.Throws<BudgetDomainException>(() => po.RecordUsage(1_500m));
    }

    [Fact]
    public void PurchaseOrder_RecordUsageOnClosedOrder_ShouldThrow()
    {
        var po = new PurchaseOrder(Guid.NewGuid(), 1_000m);
        po.RecordUsage(1_000m); // close it
        Assert.Throws<BudgetDomainException>(() => po.RecordUsage(1m));
    }

    [Fact]
    public void BudgetLine_CommittedAmount_TracksCorrectly()
    {
        var line = new BudgetLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            BudgetCategory.OpEx, "2024-01", 10_000m);
        line.UpdateAmounts(10_000m, 10_000m, 0m);
        line.UpdateCommittedAmount(4_000m);

        Assert.Equal(4_000m, line.CommittedAmount);
        Assert.Equal(6_000m, line.RemainingAmount);
        Assert.False(line.IsOverBudget);
    }

    [Fact]
    public void BudgetLine_IsOverBudget_WhenCommittedExceedsApproved()
    {
        var line = new BudgetLine(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            BudgetCategory.CapEx, "2024-01", 5_000m);
        line.UpdateAmounts(5_000m, 5_000m, 0m);
        line.UpdateCommittedAmount(6_000m);

        Assert.True(line.IsOverBudget);
    }
}
