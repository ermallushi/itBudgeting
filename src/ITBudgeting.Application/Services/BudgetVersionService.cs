using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Enums;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class BudgetVersionService
{
    private readonly IBudgetVersionRepository _versionRepo;
    private readonly IBudgetLineRepository _lineRepo;
    private readonly IBudgetPeriodRepository _periodRepo;
    private readonly IUnitOfWork _unitOfWork;

    public BudgetVersionService(
        IBudgetVersionRepository versionRepo,
        IBudgetLineRepository lineRepo,
        IBudgetPeriodRepository periodRepo,
        IUnitOfWork unitOfWork)
    {
        _versionRepo = versionRepo;
        _lineRepo = lineRepo;
        _periodRepo = periodRepo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<BudgetVersionDto>> GetAllAsync(CancellationToken ct = default)
    {
        var versions = await _versionRepo.GetAllAsync(ct);
        return versions.Select(MapToDto);
    }

    public async Task<BudgetVersionDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct);
        return version is null ? null : MapToDto(version);
    }

    public async Task<BudgetVersionDto> CreateAsync(CreateBudgetVersionDto dto, CancellationToken ct = default)
    {
        var version = new BudgetVersion(dto.Name, dto.Year, dto.Type, dto.CreatedBy, dto.ParentVersionId);
        await _versionRepo.AddAsync(version, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(version);
    }

    public async Task<BudgetVersionDto> UpdateAsync(Guid id, UpdateBudgetVersionDto dto, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget version {id} not found.");
        version.Rename(dto.Name);
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(version);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget version {id} not found.");
        if (!version.IsEditable())
            throw new BudgetDomainException("Only Draft budget versions can be deleted.");
        _versionRepo.Remove(version);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<BudgetVersionDto> SubmitAsync(Guid id, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget version {id} not found.");
        version.Submit();
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(version);
    }

    public async Task<BudgetVersionDto> ApproveAsync(Guid id, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget version {id} not found.");
        version.Approve();
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(version);
    }

    public async Task<BudgetVersionDto> LockAsync(Guid id, CancellationToken ct = default)
    {
        var version = await _versionRepo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Budget version {id} not found.");
        version.Lock();
        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(version);
    }

    public async Task<BudgetVersionDto> CloneRevisionAsync(Guid parentId, CloneRevisionDto dto, CancellationToken ct = default)
    {
        var parent = await _versionRepo.GetWithLinesAsync(parentId, ct)
            ?? throw new BudgetDomainException($"Parent budget version {parentId} not found.");

        if (parent.Status != BudgetVersionStatus.Approved && parent.Status != BudgetVersionStatus.Locked)
            throw new BudgetDomainException("Can only clone revisions from Approved or Locked versions.");

        var revision = new BudgetVersion(dto.Name, parent.Year, BudgetVersionType.Revision, dto.CreatedBy, parentId);
        await _versionRepo.AddAsync(revision, ct);

        // Clone all budget lines
        var lines = await _lineRepo.GetByVersionAsync(parentId, ct);
        foreach (var line in lines)
        {
            var clonedLine = line.Clone(revision.Id);
            await _lineRepo.AddAsync(clonedLine, ct);
        }

        // Create budget periods; only specified months are open
        for (int month = 1; month <= 12; month++)
        {
            bool isOpen = dto.OpenMonths.Contains(month);
            var lockType = isOpen ? PeriodLockType.None : PeriodLockType.FullLock;
            var period = new BudgetPeriod(revision.Id, month, parent.Year, isOpen, lockType);
            await _periodRepo.AddAsync(period, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return MapToDto(revision);
    }

    private static BudgetVersionDto MapToDto(BudgetVersion v) =>
        new(v.Id, v.Name, v.Year, v.Type, v.Status, v.ParentVersionId, v.CreatedBy, v.CreatedAt);
}
