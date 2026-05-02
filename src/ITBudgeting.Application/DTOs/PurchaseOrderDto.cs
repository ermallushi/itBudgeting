using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record PurchaseOrderDto(
    Guid Id,
    Guid PurchaseRequestId,
    decimal AmountApproved,
    decimal AmountUsed,
    decimal RemainingAmount,
    PurchaseOrderStatus Status,
    DateTime CreatedAt
);

public record CreatePurchaseOrderDto(Guid PurchaseRequestId, decimal AmountApproved);
public record RecordUsageDto(decimal Amount);
