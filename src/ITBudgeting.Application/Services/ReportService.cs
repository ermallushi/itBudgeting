using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Interfaces;
using ITBudgeting.Domain.Entities;

namespace ITBudgeting.Application.Services;

public class ReportService
{
    private readonly IBudgetLineRepository _lineRepo;
    private readonly IRepository<Project> _projectRepo;
    private readonly IRepository<CostCenter> _costCenterRepo;

    public ReportService(
        IBudgetLineRepository lineRepo,
        IRepository<Project> projectRepo,
        IRepository<CostCenter> costCenterRepo)
    {
        _lineRepo = lineRepo;
        _projectRepo = projectRepo;
        _costCenterRepo = costCenterRepo;
    }

    public async Task<IEnumerable<BudgetSummaryLineDto>> GetSummaryAsync(Guid versionId, CancellationToken ct = default)
    {
        var lines = await _lineRepo.GetByVersionAsync(versionId, ct);
        var projects = (await _projectRepo.GetAllAsync(ct)).ToDictionary(p => p.Id);
        var costCenters = (await _costCenterRepo.GetAllAsync(ct)).ToDictionary(c => c.Id);

        return lines.Select(l =>
        {
            projects.TryGetValue(l.ProjectId, out var project);
            costCenters.TryGetValue(l.CostCenterId, out var costCenter);
            return new BudgetSummaryLineDto(
                l.ProjectId,
                project?.Name ?? "Unknown",
                l.CostCenterId,
                costCenter?.Name ?? "Unknown",
                l.Category,
                l.Period,
                l.PlannedAmount,
                l.ApprovedAmount,
                l.CommittedAmount,
                l.ActualAmount,
                l.RemainingAmount,
                l.IsOverBudget
            );
        });
    }

    public async Task<IEnumerable<BudgetVarianceLineDto>> GetVarianceAsync(Guid versionId, CancellationToken ct = default)
    {
        var lines = await _lineRepo.GetByVersionAsync(versionId, ct);
        var projects = (await _projectRepo.GetAllAsync(ct)).ToDictionary(p => p.Id);

        return lines.Select(l =>
        {
            projects.TryGetValue(l.ProjectId, out var project);
            var variance = l.ApprovedAmount - l.ActualAmount;
            var variancePct = l.ApprovedAmount != 0
                ? Math.Round(variance / l.ApprovedAmount * 100, 2)
                : 0m;
            return new BudgetVarianceLineDto(
                l.ProjectId,
                project?.Name ?? "Unknown",
                l.Period,
                l.ApprovedAmount,
                l.ActualAmount,
                variance,
                variancePct
            );
        });
    }
}
