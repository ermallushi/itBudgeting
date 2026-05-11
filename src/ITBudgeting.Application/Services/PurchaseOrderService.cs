using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class PurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repo;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseOrderService(IPurchaseOrderRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PurchaseOrderDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<PurchaseOrderDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<PurchaseOrderDto> CreateAsync(CreatePurchaseOrderDto dto, CancellationToken ct = default)
    {
        var entity = new PurchaseOrder(dto.PurchaseRequestId, dto.AmountApproved);
        await _repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<PurchaseOrderDto> RecordUsageAsync(Guid id, RecordUsageDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Purchase order {id} not found.");
        entity.RecordUsage(dto.Amount);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Purchase order {id} not found.");
        _repo.Remove(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static PurchaseOrderDto MapToDto(PurchaseOrder po) =>
        new(po.Id, po.PurchaseRequestId, po.AmountApproved, po.AmountUsed, po.RemainingAmount, po.Status, po.CreatedAt);
}
