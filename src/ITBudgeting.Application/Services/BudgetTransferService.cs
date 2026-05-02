using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class BudgetTransferService
{
    private readonly IBudgetTransferRepository _transferRepo;
    private readonly IBudgetLineRepository _lineRepo;
    private readonly IBudgetPeriodRepository _periodRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BudgetTransferService(
        IBudgetTransferRepository transferRepo,
        IBudgetLineRepository lineRepo,
        IBudgetPeriodRepository periodRepo,
        IUnitOfWork unitOfWork)
    {
        _transferRepo = transferRepo;
        _lineRepo = lineRepo;
        _periodRepo = periodRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetTransferDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _transferRepo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<BudgetTransferDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _transferRepo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<BudgetTransferDto> CreateAsync(CreateBudgetTransferDto dto, CancellationToken ct = default)
    {
        // Validate period is open
        var period = await _periodRepo.GetByVersionAndPeriodAsync(dto.VersionId, dto.Period, ct);
        if (period is not null && period.IsLocked())
            throw new BudgetDomainException($"Period '{dto.Period}' is locked. Transfers are not allowed.");

        // Validate source project has sufficient available budget
        var sourceLine = await _lineRepo.GetByProjectAndPeriodAsync(dto.VersionId, dto.FromProjectId, dto.Period, ct)
            ?? throw new BudgetDomainException($"No budget line found for source project in period '{dto.Period}'.");

        var existingTransfers = await _transferRepo.GetTotalTransferredFromProjectAsync(dto.VersionId, dto.FromProjectId, dto.Period, ct);
        var available = sourceLine.ApprovedAmount - sourceLine.CommittedAmount - existingTransfers;

        if (dto.Amount > available)
            throw new BudgetDomainException(
                $"Insufficient budget. Available: {available:C}, Requested: {dto.Amount:C}.");

        var transfer = new BudgetTransfer(
            dto.FromProjectId, dto.ToProjectId, dto.Amount,
            dto.Category, dto.Period, dto.Reason, dto.ApprovedBy, dto.CreatedBy);

        await _transferRepo.AddAsync(transfer, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(transfer);
    }

    private static BudgetTransferDto MapToDto(BudgetTransfer t) =>
        new(t.Id, t.FromProjectId, t.ToProjectId, t.Amount, t.Category,
            t.Period, t.Reason, t.ApprovedBy, t.CreatedBy, t.CreatedAt);
}
