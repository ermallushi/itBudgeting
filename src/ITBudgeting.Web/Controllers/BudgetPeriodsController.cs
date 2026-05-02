using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class BudgetPeriodsController : Controller
{
    private readonly BudgetPeriodService _svc;
    private readonly BudgetVersionService _versionSvc;

    public BudgetPeriodsController(BudgetPeriodService svc, BudgetVersionService versionSvc)
    {
        _svc        = svc;
        _versionSvc = versionSvc;
    }

    public async Task<IActionResult> Index(Guid? versionId, CancellationToken ct)
    {
        var all = await _svc.GetAllAsync(ct);
        var periods = versionId.HasValue ? all.Where(p => p.VersionId == versionId) : all;
        ViewBag.VersionId = versionId;
        if (versionId.HasValue)
        {
            var v = await _versionSvc.GetByIdAsync(versionId.Value, ct);
            ViewBag.VersionName = v?.Name;
        }
        return View(periods);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        ViewBag.PeriodId = id;
        ViewBag.Month    = item.Month;
        ViewBag.Year     = item.Year;
        ViewBag.VersionId = item.VersionId;
        return View(new EditBudgetPeriodViewModel { IsOpen = item.IsOpen, LockType = item.LockType });
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Edit(Guid id, EditBudgetPeriodViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { ViewBag.PeriodId = id; return View(vm); }
        try
        {
            var period = await _svc.GetByIdAsync(id, ct);
            await _svc.UpdateAsync(id, new UpdateBudgetPeriodDto(vm.IsOpen, vm.LockType), ct);
            TempData["Success"] = "Period updated.";
            return RedirectToAction(nameof(Index), new { versionId = period?.VersionId });
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; return View(vm); }
    }
}
