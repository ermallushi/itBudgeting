using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class BudgetPeriodService
{
    private readonly IBudgetPeriodRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public BudgetPeriodService(IBudgetPeriodRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetPeriodDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<BudgetPeriodDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<BudgetPeriodDto> CreateAsync(CreateBudgetPeriodDto dto, CancellationToken ct = default)
    {
        var entity = new BudgetPeriod(dto.VersionId, dto.Month, dto.Year, dto.IsOpen, dto.LockType);
        await _repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<BudgetPeriodDto> UpdateAsync(Guid id, UpdateBudgetPeriodDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget period {id} not found.");

        if (dto.IsOpen)
            entity.Open();
        else
            entity.Close(dto.LockType);

        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    private static BudgetPeriodDto MapToDto(BudgetPeriod p) =>
        new(p.Id, p.VersionId, p.Month, p.Year, p.IsOpen, p.LockType);
}
