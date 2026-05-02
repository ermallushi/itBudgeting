using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;
using ITBudgeting.Domain.Exceptions;

namespace ITBudgeting.Application.Services;

public class ProjectService
{
    private readonly IRepository<Project> _repo;
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IRepository<Project> repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);
        return items.Select(p => new ProjectDto(p.Id, p.Name, p.CostCenterId, p.Category, p.IsActive));
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _repo.GetByIdAsync(id, ct);
        return item is null ? null : new ProjectDto(item.Id, item.Name, item.CostCenterId, item.Category, item.IsActive);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectDto dto, CancellationToken ct = default)
    {
        var entity = new Project(dto.Name, dto.CostCenterId, dto.Category);
        await _repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return new ProjectDto(entity.Id, entity.Name, entity.CostCenterId, entity.Category, entity.IsActive);
    }

    public async Task<ProjectDto> UpdateAsync(Guid id, UpdateProjectDto dto, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Project {id} not found.");
        entity.Update(dto.Name, dto.Category, dto.IsActive);
        await _unitOfWork.SaveChangesAsync(ct);
        return new ProjectDto(entity.Id, entity.Name, entity.CostCenterId, entity.Category, entity.IsActive);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repo.GetByIdAsync(id, ct)
            ?? throw new BudgetDomainException($"Project {id} not found.");
        _repo.Remove(entity);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
