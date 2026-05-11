using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class ProjectsController : Controller
{
    private readonly ProjectService _svc;
    private readonly CostCenterService _costCenterSvc;

    public ProjectsController(ProjectService svc, CostCenterService costCenterSvc)
    {
        _svc = svc;
        _costCenterSvc = costCenterSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _svc.GetAllAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreateProjectViewModel();
        await PopulateCostCenters(vm, ct);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateProjectViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCostCenters(vm, ct);
            return View(vm);
        }
        try
        {
            await _svc.CreateAsync(new CreateProjectDto(vm.Name, vm.CostCenterId, vm.Category), ct);
            TempData["Success"] = "Project created.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex)
        {
            TempData["Error"] = ex.Message;
            await PopulateCostCenters(vm, ct);
            return View(vm);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        var item = await _svc.GetByIdAsync(id, ct);
        if (item is null) return NotFound();
        return View(new EditProjectViewModel { Name = item.Name, Category = item.Category, IsActive = item.IsActive });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditProjectViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);
        try
        {
            await _svc.UpdateAsync(id, new UpdateProjectDto(vm.Name, vm.Category, vm.IsActive), ct);
            TempData["Success"] = "Project updated.";
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
            TempData["Success"] = "Project deleted.";
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    private async Task PopulateCostCenters(CreateProjectViewModel vm, CancellationToken ct)
    {
        var items = await _costCenterSvc.GetAllAsync(ct);
        vm.CostCenters = items.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
    }
}
