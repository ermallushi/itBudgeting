using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class PurchaseRequestsController : Controller
{
    private readonly PurchaseRequestService _svc;
    private readonly ProjectService _projectSvc;
    private readonly CostCenterService _costCenterSvc;

    public PurchaseRequestsController(PurchaseRequestService svc, ProjectService projectSvc, CostCenterService costCenterSvc)
    {
        _svc           = svc;
        _projectSvc    = projectSvc;
        _costCenterSvc = costCenterSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _svc.GetAllAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreatePurchaseRequestViewModel();
        await PopulateDropdowns(vm, ct);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseRequestViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await PopulateDropdowns(vm, ct); return View(vm); }
        try
        {
            var dto = new CreatePurchaseRequestDto(vm.ProjectId, vm.CostCenterId, vm.Amount, User.Identity!.Name!, vm.SharePointUrl);
            await _svc.CreateAsync(dto, ct);
            TempData["Success"] = "Purchase request created.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; await PopulateDropdowns(vm, ct); return View(vm); }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        return View(new EditPurchaseRequestViewModel { Amount = item.Amount, SharePointUrl = item.SharePointUrl });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditPurchaseRequestViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            await _svc.UpdateAsync(id, new UpdatePurchaseRequestDto(vm.Amount, vm.SharePointUrl), ct);
            TempData["Success"] = "Purchase request updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; return View(vm); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try { await _svc.DeleteAsync(id, ct); TempData["Success"] = "Purchase request deleted."; }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        try { await _svc.ApproveAsync(id, ct); TempData["Success"] = "Purchase request approved and PO created."; }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateDropdowns(CreatePurchaseRequestViewModel vm, CancellationToken ct)
    {
        var projects    = await _projectSvc.GetAllAsync(ct);
        var costCenters = await _costCenterSvc.GetAllAsync(ct);
        vm.Projects     = projects.Select(p => new SelectListItem(p.Name, p.Id.ToString()));
        vm.CostCenters  = costCenters.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
    }
}
