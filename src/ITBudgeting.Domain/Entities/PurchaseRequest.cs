using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Domain.Entities;

public class PurchaseRequest
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid CostCenterId { get; private set; }
    public decimal Amount { get; private set; }
    public PurchaseRequestStatus Status { get; private set; }
    public string? SharePointUrl { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public Project? Project { get; private set; }
    public CostCenter? CostCenter { get; private set; }
    public PurchaseOrder? PurchaseOrder { get; private set; }

    private PurchaseRequest() { }

    public PurchaseRequest(Guid projectId, Guid costCenterId, decimal amount, string createdBy, string? sharePointUrl = null)
    {
        Id = Guid.NewGuid();
        ProjectId = projectId;
        CostCenterId = costCenterId;
        Amount = amount;
        Status = PurchaseRequestStatus.Draft;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        SharePointUrl = sharePointUrl;
    }

    public void Submit()
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new BudgetDomainException("Only Draft purchase requests can be submitted.");
        Status = PurchaseRequestStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != PurchaseRequestStatus.Submitted)
            throw new BudgetDomainException("Only Submitted purchase requests can be approved.");
        Status = PurchaseRequestStatus.Approved;
    }

    public void Reject()
    {
        if (Status == PurchaseRequestStatus.Approved)
            throw new BudgetDomainException("Approved purchase requests cannot be rejected.");
        Status = PurchaseRequestStatus.Rejected;
    }

    public void Update(decimal amount, string? sharePointUrl)
    {
        if (Status != PurchaseRequestStatus.Draft)
            throw new BudgetDomainException("Only Draft purchase requests can be edited.");
        Amount = amount;
        SharePointUrl = sharePointUrl;
    }
}
