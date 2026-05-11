namespace ITBudgeting.Domain.Entities;

public class CostCenter
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;

    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    public ICollection<BudgetLine> BudgetLines { get; private set; } = new List<BudgetLine>();

    private CostCenter() { }

    public CostCenter(string name, string department)
    {
        Id = Guid.NewGuid();
        Name = name;
        Department = department;
    }

    public void Update(string name, string department)
    {
        Name = name;
        Department = department;
    }
}
