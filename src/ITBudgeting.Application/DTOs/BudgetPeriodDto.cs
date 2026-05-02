using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record BudgetPeriodDto(
    Guid Id,
    Guid VersionId,
    int Month,
    int Year,
    bool IsOpen,
    PeriodLockType LockType
);

public record CreateBudgetPeriodDto(Guid VersionId, int Month, int Year, bool IsOpen = true, PeriodLockType LockType = PeriodLockType.None);
public record UpdateBudgetPeriodDto(bool IsOpen, PeriodLockType LockType);
