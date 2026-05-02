using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class BudgetTransfersController : Controller
{
    private readonly BudgetTransferService _svc;
    private readonly ProjectService _projectSvc;
    private readonly BudgetVersionService _versionSvc;

    public BudgetTransfersController(BudgetTransferService svc, ProjectService projectSvc, BudgetVersionService versionSvc)
    {
        _svc        = svc;
        _projectSvc = projectSvc;
        _versionSvc = versionSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await _svc.GetAllAsync(ct));

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var vm = new CreateBudgetTransferViewModel();
        await PopulateDropdowns(vm, ct);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateBudgetTransferViewModel vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) { await PopulateDropdowns(vm, ct); return View(vm); }
        try
        {
            var dto = new CreateBudgetTransferDto(vm.VersionId, vm.FromProjectId, vm.ToProjectId,
                vm.Amount, vm.Category, vm.Period, vm.Reason, vm.ApprovedBy, User.Identity!.Name!);
            await _svc.CreateAsync(dto, ct);
            TempData["Success"] = "Budget transfer recorded.";
            return RedirectToAction(nameof(Index));
        }
        catch (BudgetDomainException ex) { TempData["Error"] = ex.Message; await PopulateDropdowns(vm, ct); return View(vm); }
    }

    private async Task PopulateDropdowns(CreateBudgetTransferViewModel vm, CancellationToken ct)
    {
        var projects = await _projectSvc.GetAllAsync(ct);
        var versions = await _versionSvc.GetAllAsync(ct);
        vm.Projects  = projects.Select(p => new SelectListItem(p.Name, p.Id.ToString()));
        vm.Versions  = versions.Select(v => new SelectListItem($"{v.Name} ({v.Year})", v.Id.ToString()));
    }
}
