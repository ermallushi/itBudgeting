using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Domain.Entities;

public class PurchaseOrder
{
    public Guid Id { get; private set; }
    public Guid PurchaseRequestId { get; private set; }
    public decimal AmountApproved { get; private set; }
    public decimal AmountUsed { get; private set; }
    public decimal RemainingAmount => AmountApproved - AmountUsed;
    public PurchaseOrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public PurchaseRequest? PurchaseRequest { get; private set; }

    private PurchaseOrder() { }

    public PurchaseOrder(Guid purchaseRequestId, decimal amountApproved)
    {
        Id = Guid.NewGuid();
        PurchaseRequestId = purchaseRequestId;
        AmountApproved = amountApproved;
        AmountUsed = 0;
        Status = PurchaseOrderStatus.Open;
        CreatedAt = DateTime.UtcNow;
    }

    public void RecordUsage(decimal amount)
    {
        if (Status == PurchaseOrderStatus.Closed)
            throw new BudgetDomainException("Cannot record usage on a closed purchase order.");
        if (amount <= 0)
            throw new BudgetDomainException("Usage amount must be positive.");
        if (AmountUsed + amount > AmountApproved)
            throw new BudgetDomainException($"Usage amount exceeds remaining balance of {RemainingAmount:C}.");

        AmountUsed += amount;
        Status = AmountUsed >= AmountApproved
            ? PurchaseOrderStatus.Closed
            : PurchaseOrderStatus.PartiallyUsed;
    }

    public void Close()
    {
        Status = PurchaseOrderStatus.Closed;
    }
}
