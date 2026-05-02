using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Domain.Entities;

public class BudgetPeriod
{
    public Guid Id { get; private set; }
    public Guid VersionId { get; private set; }
    public int Month { get; private set; }
    public int Year { get; private set; }
    public bool IsOpen { get; private set; }
    public PeriodLockType LockType { get; private set; }

    public BudgetVersion? Version { get; private set; }

    private BudgetPeriod() { }

    public BudgetPeriod(Guid versionId, int month, int year, bool isOpen = true, PeriodLockType lockType = PeriodLockType.None)
    {
        Id = Guid.NewGuid();
        VersionId = versionId;
        Month = month;
        Year = year;
        IsOpen = isOpen;
        LockType = lockType;
    }

    public void Open()
    {
        IsOpen = true;
        LockType = PeriodLockType.None;
    }

    public void Close(PeriodLockType lockType = PeriodLockType.FullLock)
    {
        IsOpen = false;
        LockType = lockType;
    }

    public bool IsLocked() => !IsOpen || LockType == PeriodLockType.FullLock;

    public string ToPeriodString() => $"{Year:D4}-{Month:D2}";
}
