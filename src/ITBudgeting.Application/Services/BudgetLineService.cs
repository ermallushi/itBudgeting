using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class BudgetLineService
{
    private readonly IBudgetLineRepository _lineRepo;
    private readonly IBudgetVersionRepository _versionRepo;
    private readonly IBudgetPeriodRepository _periodRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BudgetLineService(
        IBudgetLineRepository lineRepo,
        IBudgetVersionRepository versionRepo,
        IBudgetPeriodRepository periodRepo,
        IUnitOfWork unitOfWork)
    {
        _lineRepo = lineRepo;
        _versionRepo = versionRepo;
        _periodRepo = periodRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetLineDto>> GetByFilterAsync(
        Guid? versionId, Guid? costCenterId, Guid? projectId, CancellationToken ct = default)
    {
        var lines = await _lineRepo.GetByFilterAsync(versionId, costCenterId, projectId, ct);
        return lines.Select(MapToDto);
    }

    public async Task<BudgetLineDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var line = await _lineRepo.GetByIdAsync(id, ct);
        return line is null ? null : MapToDto(line);
    }

    public async Task<BudgetLineDto> CreateAsync(CreateBudgetLineDto dto, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(dto.VersionId, ct)
            ?? throw new BudgetDomainException($"Budget version {dto.VersionId} not found.");
        if (!version.IsEditable())
            throw new BudgetDomainException("Cannot add lines to a non-Draft budget version.");

        await ValidatePeriodNotLocked(dto.VersionId, dto.Period, ct);

        var line = new BudgetLine(dto.VersionId, dto.CostCenterId, dto.ProjectId, dto.Category, dto.Period, dto.PlannedAmount);
        await _lineRepo.AddAsync(line, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(line);
    }

    public async Task<BudgetLineDto> UpdateAsync(Guid id, UpdateBudgetLineDto dto, CancellationToken ct = default)
    {
        var line = await _lineRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget line {id} not found.");

        var version = await _versionRepo.GetByIdAsync(line.VersionId, ct)
            ?? throw new BudgetDomainException($"Budget version not found.");
        if (!version.IsEditable())
            throw new BudgetDomainException("Cannot edit lines in a non-Draft budget version.");

        await ValidatePeriodNotLocked(line.VersionId, line.Period, ct);

        line.UpdateAmounts(dto.PlannedAmount, dto.ApprovedAmount, dto.ActualAmount);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(line);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var line = await _lineRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget line {id} not found.");
        var version = await _versionRepo.GetByIdAsync(line.VersionId, ct)
            ?? throw new BudgetDomainException($"Budget version not found.");
        if (!version.IsEditable())
            throw new BudgetDomainException("Cannot delete lines from a non-Draft budget version.");
        _lineRepo.Remove(line);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task ValidatePeriodNotLocked(Guid versionId, string period, CancellationToken ct)
    {
        var budgetPeriod = await _periodRepo.GetByVersionAndPeriodAsync(versionId, period, ct);
        if (budgetPeriod is not null && budgetPeriod.IsLocked())
            throw new BudgetDomainException($"Period '{period}' is locked and cannot be edited.");
    }

    private static BudgetLineDto MapToDto(BudgetLine l) =>
        new(l.Id, l.VersionId, l.CostCenterId, l.ProjectId, l.Category, l.Period,
            l.PlannedAmount, l.ApprovedAmount, l.CommittedAmount, l.ActualAmount,
            l.RemainingAmount, l.IsOverBudget);
}
