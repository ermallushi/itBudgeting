using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class PurchaseOrdersController : Controller
{
    private readonly PurchaseOrderService _svc;
    private readonly PurchaseRequestService _prSvc;

    public PurchaseOrdersController(PurchaseOrderService svc, PurchaseRequestService prSvc)
    {
        _svc   = svc;
        _prSvc = prSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _svc.GetAllAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreatePurchaseOrderViewModel();
        await PopulateDropdowns(vm, ct);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await PopulateDropdowns(vm, ct); return View(vm); }
        try
        {
            await _svc.CreateAsync(new CreatePurchaseOrderDto(vm.PurchaseRequestId, vm.AmountApproved), ct);
            TempData["Success"] = "Purchase order created.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; await PopulateDropdowns(vm, ct); return View(vm); }
    }

    [HttpGet]
    public async Task<IActionResult> RecordUsage(Guid id, CancellationToken ct)
    {
        var po = await _svc.GetByIdAsync(id, ct);
        if (po is null) return NotFound();
        ViewBag.PoId            = id;
        ViewBag.RemainingAmount = po.RemainingAmount;
        return View(new RecordUsageViewModel());
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RecordUsage(Guid id, RecordUsageViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { ViewBag.PoId = id; return View(vm); }
        try
        {
            await _svc.RecordUsageAsync(id, new RecordUsageDto(vm.Amount), ct);
            TempData["Success"] = "Usage recorded.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; ViewBag.PoId = id; return View(vm); }
    }

    private async Task PopulateDropdowns(CreatePurchaseOrderViewModel vm, CancellationToken ct)
    {
        var prs = await _prSvc.GetAllAsync(ct);
        vm.PurchaseRequests = prs.Select(p => new SelectListItem($"PR {p.Id.ToString()[..8]}… ({p.Amount:C})", p.Id.ToString()));
    }
}
