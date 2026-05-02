using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Guid CostCenterId { get; private set; }
    public BudgetCategory Category { get; private set; }
    public bool IsActive { get; private set; }

    public CostCenter? CostCenter { get; private set; }
    public ICollection<BudgetLine> BudgetLines { get; private set; } = new List<BudgetLine>();

    private Project() { }

    public Project(string name, Guid costCenterId, BudgetCategory category)
    {
        Id = Guid.NewGuid();
        Name = name;
        CostCenterId = costCenterId;
        Category = category;
        IsActive = true;
    }

    public void Update(string name, BudgetCategory category, bool isActive)
    {
        Name = name;
        Category = category;
        IsActive = isActive;
    }
}
