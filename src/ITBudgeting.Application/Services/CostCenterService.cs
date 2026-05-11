using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class CostCenterService
{
    private readonly IRepository<CostCenter> _repo;
    private readonly IUnitOfWork _unitOfWork;

    public CostCenterService(IRepository<CostCenter> repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CostCenterDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(c => new CostCenterDto(c.Id, c.Name, c.Department));
    }

    public async Task<CostCenterDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : new CostCenterDto(item.Id, item.Name, item.Department);
    }

    public async Task<CostCenterDto> CreateAsync(CreateCostCenterDto dto, CancellationToken ct = default)
    {
        var entity = new CostCenter(dto.Name, dto.Department);
        await _repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new CostCenterDto(entity.Id, entity.Name, entity.Department);
    }

    public async Task<CostCenterDto> UpdateAsync(Guid id, UpdateCostCenterDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Cost center {id} not found.");
        entity.Update(dto.Name, dto.Department);
        await _unitOfWork.SaveChangesAsync(ct);
        return new CostCenterDto(entity.Id, entity.Name, entity.Department);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Cost center {id} not found.");
        _repo.Remove(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
