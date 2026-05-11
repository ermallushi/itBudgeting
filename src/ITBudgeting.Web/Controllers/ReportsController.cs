using ITBudgeting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class ReportsController : Controller
{
    private readonly ReportService _svc;
    private readonly BudgetVersionService _versionSvc;

    public ReportsController(ReportService svc, BudgetVersionService versionSvc)
    {
        _svc        = svc;
        _versionSvc = versionSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.Versions = await _versionSvc.GetAllAsync(ct);
        return View();
    }

    public async Task<IActionResult> Summary(Guid versionId, CancellationToken ct)
    {
        var data = await _svc.GetSummaryAsync(versionId, ct);
        var ver  = await _versionSvc.GetByIdAsync(versionId, ct);
        ViewBag.VersionName = ver?.Name;
        ViewBag.VersionId   = versionId;
        return View(data);
    }

    public async Task<IActionResult> Variance(Guid versionId, CancellationToken ct)
    {
        var data = await _svc.GetVarianceAsync(versionId, ct);
        var ver  = await _versionSvc.GetByIdAsync(versionId, ct);
        ViewBag.VersionName = ver?.Name;
        ViewBag.VersionId   = versionId;
        return View(data);
    }
}
