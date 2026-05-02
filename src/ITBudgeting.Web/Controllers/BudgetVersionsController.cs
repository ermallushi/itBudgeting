using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class BudgetVersionsController : Controller
{
    private readonly BudgetVersionService _svc;

    public BudgetVersionsController(BudgetVersionService svc) => _svc = svc;

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var versions = await _svc.GetAllAsync(ct);
        return View(versions);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateBudgetVersionViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetVersionViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            var dto = new CreateBudgetVersionDto(vm.Name, vm.Year, vm.Type, User.Identity!.Name!, vm.ParentVersionId);
            await _svc.CreateAsync(dto, ct);
            TempData["Success"] = "Budget version created.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var v = await _svc.GetByIdAsync(id, ct);
        if (v is null) return NotFound();
        return View(new EditBudgetVersionViewModel { Name = v.Name });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditBudgetVersionViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            await _svc.UpdateAsync(id, new UpdateBudgetVersionDto(vm.Name), ct);
            TempData["Success"] = "Budget version updated.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
        }
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.DeleteAsync(id, ct);
            TempData["Success"] = "Budget version deleted.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.SubmitAsync(id, ct);
            TempData["Success"] = "Budget version submitted for approval.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.ApproveAsync(id, ct);
            TempData["Success"] = "Budget version approved.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Lock(Guid id, CancellationToken ct)
    {
        try
        {
            await _svc.LockAsync(id, ct);
            TempData["Success"] = "Budget version locked.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> CloneRevision(Guid id, CancellationToken ct)
    {
        var v = await _svc.GetByIdAsync(id, ct);
        if (v is null) return NotFound();
        return View(new CloneRevisionViewModel { ParentId = id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CloneRevision(Guid id, CloneRevisionViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            var openMonths = Enumerable.Range(0, 12)
                .Where(i => vm.OpenMonths[i])
                .Select(i => i + 1)
                .ToList();
            var dto = new CloneRevisionDto(vm.Name, User.Identity!.Name!, openMonths);
            await _svc.CloneRevisionAsync(id, dto, ct);
            TempData["Success"] = "Revision cloned successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            return View(vm);
        }
    }
}
