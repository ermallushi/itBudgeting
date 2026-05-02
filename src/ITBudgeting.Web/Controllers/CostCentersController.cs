using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class CostCentersController : Controller
{
    private readonly CostCenterService _svc;
    public CostCentersController(CostCenterService svc) => _svc = svc;

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _svc.GetAllAsync(ct));

    [HttpGet]
    public IActionResult Create() => View(new CreateCostCenterViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCostCenterViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            await _svc.CreateAsync(new CreateCostCenterDto(vm.Name, vm.Department), ct);
            TempData["Success"] = "Cost center created.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; return View(vm); }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        return View(new EditCostCenterViewModel { Name = item.Name, Department = item.Department });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditCostCenterViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            await _svc.UpdateAsync(id, new UpdateCostCenterDto(vm.Name, vm.Department), ct);
            TempData["Success"] = "Cost center updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; return View(vm); }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteAsync(id, ct);
            TempData["Success"] = "Cost center deleted.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
