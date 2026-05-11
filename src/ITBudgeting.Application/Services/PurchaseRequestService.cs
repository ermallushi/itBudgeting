using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class PurchaseRequestService
{
    private readonly IRepository<PurchaseRequest> _repo;
    private readonly IRepository<PurchaseOrder> _poRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PurchaseRequestService(
        IRepository<PurchaseRequest> repo,
        IRepository<PurchaseOrder> poRepo,
        IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _poRepo = poRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<PurchaseRequestDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(MapToDto);
    }

    public async Task<PurchaseRequestDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : MapToDto(item);
    }

    public async Task<PurchaseRequestDto> CreateAsync(CreatePurchaseRequestDto dto, CancellationToken ct = default)
    {
        var entity = new PurchaseRequest(dto.ProjectId, dto.CostCenterId, dto.Amount, dto.CreatedBy, dto.SharePointUrl);
        await _repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<PurchaseRequestDto> UpdateAsync(Guid id, UpdatePurchaseRequestDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Purchase request {id} not found.");
        entity.Update(dto.Amount, dto.SharePointUrl);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Purchase request {id} not found.");
        _repo.Remove(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<PurchaseRequestDto> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Purchase request {id} not found.");
        entity.Approve();

        // Create purchase order automatically
        var po = new PurchaseOrder(entity.Id, entity.Amount);
        await _poRepo.AddAsync(po, ct);

        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    private static PurchaseRequestDto MapToDto(PurchaseRequest pr) =>
        new(pr.Id, pr.ProjectId, pr.CostCenterId, pr.Amount, pr.Status, pr.SharePointUrl, pr.CreatedBy, pr.CreatedAt);
}
