using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record PurchaseRequestDto(
    Guid Id,
    Guid ProjectId,
    Guid CostCenterId,
    decimal Amount,
    PurchaseRequestStatus Status,
    string? SharePointUrl,
    string CreatedBy,
    DateTime CreatedAt
);

public record CreatePurchaseRequestDto(
    Guid ProjectId,
    Guid CostCenterId,
    decimal Amount,
    string CreatedBy,
    string? SharePointUrl = null
);

public record UpdatePurchaseRequestDto(decimal Amount, string? SharePointUrl);
