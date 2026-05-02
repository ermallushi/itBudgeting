using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class BudgetLinesController : Controller
{
    private readonly BudgetLineService _lineSvc;
    private readonly BudgetVersionService _versionSvc;
    private readonly CostCenterService _costCenterSvc;
    private readonly ProjectService _projectSvc;

    public BudgetLinesController(
        BudgetLineService lineSvc,
        BudgetVersionService versionSvc,
        CostCenterService costCenterSvc,
        ProjectService projectSvc)
    {
        _lineSvc       = lineSvc;
        _versionSvc    = versionSvc;
        _costCenterSvc = costCenterSvc;
        _projectSvc    = projectSvc;
    }

    public async Task<IActionResult> Index(Guid? versionId, CancellationToken ct)
    {
        var lines = await _lineSvc.GetByFilterAsync(versionId, null, null, ct);
        ViewBag.VersionId = versionId;
        if (versionId.HasValue)
        {
            var ver = await _versionSvc.GetByIdAsync(versionId.Value, ct);
            ViewBag.VersionName = ver?.Name;
        }
        return View(lines);
    }

    [HttpGet]
    public async Task<IActionResult> Create(Guid versionId, CancellationToken ct)
    {
        var vm = new CreateBudgetLineViewModel { VersionId = versionId };
        await PopulateDropdowns(vm, ct);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetLineViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(vm, ct);
            return View(vm);
        }
        try
        {
            var dto = new CreateBudgetLineDto(vm.VersionId, vm.CostCenterId, vm.ProjectId, vm.Category, vm.Period, vm.PlannedAmount);
            await _lineSvc.CreateAsync(dto, ct);
            TempData["Success"] = "Budget line created.";
            return RedirectToAction(nameof(Index), new { versionId = vm.VersionId });
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            await PopulateDropdowns(vm, ct);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var line = await _lineSvc.GetByIdAsync(id, ct);
        if (line is null) return NotFound();
        ViewBag.LineId     = id;
        ViewBag.VersionId  = line.VersionId;
        return View(new EditBudgetLineViewModel
        {
            PlannedAmount  = line.PlannedAmount,
            ApprovedAmount = line.ApprovedAmount,
            ActualAmount   = line.ActualAmount,
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditBudgetLineViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { ViewBag.LineId = id; return View(vm); }
        try
        {
            var line = await _lineSvc.GetByIdAsync(id, ct);
            var dto  = new UpdateBudgetLineDto(vm.PlannedAmount, vm.ApprovedAmount, vm.ActualAmount);
            await _lineSvc.UpdateAsync(id, dto, ct);
            TempData["Success"] = "Budget line updated.";
            return RedirectToAction(nameof(Index), new { versionId = line?.VersionId });
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid versionId, CancellationToken ct)
    {
        try
        {
            await _lineSvc.DeleteAsync(id, ct);
            TempData["Success"] = "Budget line deleted.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index), new { versionId });
    }

    private async Task PopulateDropdowns(CreateBudgetLineViewModel vm, CancellationToken ct)
    {
        var costCenters = await _costCenterSvc.GetAllAsync(ct);
        var projects    = await _projectSvc.GetAllAsync(ct);
        vm.CostCenters  = costCenters.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
        vm.Projects     = projects.Select(p => new SelectListItem(p.Name, p.Id.ToString()));
    }
}
