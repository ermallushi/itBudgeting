using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Domain.Entities;

public class BudgetVersion
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public BudgetVersionType Type { get; private set; }
    public BudgetVersionStatus Status { get; private set; }
    public Guid? ParentVersionId { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public ICollection<BudgetLine> BudgetLines { get; private set; } = new List<BudgetLine>();
    public ICollection<BudgetPeriod> BudgetPeriods { get; private set; } = new List<BudgetPeriod>();

    private BudgetVersion() { }

    public BudgetVersion(string name, int year, BudgetVersionType type, string createdBy, Guid? parentVersionId = null)
    {
        Id = Guid.NewGuid();
        Name = name;
        Year = year;
        Type = type;
        Status = BudgetVersionStatus.Draft;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
        ParentVersionId = parentVersionId;
    }

    public void Submit()
    {
        if (Status != BudgetVersionStatus.Draft)
            throw new BudgetDomainException($"Cannot submit version in status '{Status}'. Only Draft versions can be submitted.");
        Status = BudgetVersionStatus.Submitted;
    }

    public void Approve()
    {
        if (Status != BudgetVersionStatus.Submitted)
            throw new BudgetDomainException($"Cannot approve version in status '{Status}'. Only Submitted versions can be approved.");
        Status = BudgetVersionStatus.Approved;
    }

    public void Lock()
    {
        if (Status != BudgetVersionStatus.Approved)
            throw new BudgetDomainException($"Cannot lock version in status '{Status}'. Only Approved versions can be locked.");
        Status = BudgetVersionStatus.Locked;
    }

    public void Rename(string name)
    {
        if (!IsEditable())
            throw new BudgetDomainException("Only Draft budget versions can be renamed.");
        Name = name;
    }

    public bool IsEditable() => Status == BudgetVersionStatus.Draft;
}
