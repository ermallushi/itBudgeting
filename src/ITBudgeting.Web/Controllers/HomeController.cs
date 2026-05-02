using ITBudgeting.Application.Services;
using ITBudgeting.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly BudgetVersionService _versionSvc;
    private readonly CostCenterService _costCenterSvc;
    private readonly ProjectService _projectSvc;
    private readonly PurchaseRequestService _prSvc;
    private readonly PurchaseOrderService _poSvc;
    private readonly BudgetTransferService _transferSvc;
    private readonly BudgetLineService _lineSvc;

    public HomeController(
        BudgetVersionService versionSvc,
        CostCenterService costCenterSvc,
        ProjectService projectSvc,
        PurchaseRequestService prSvc,
        PurchaseOrderService poSvc,
        BudgetTransferService transferSvc,
        BudgetLineService lineSvc)
    {
        _versionSvc  = versionSvc;
        _costCenterSvc = costCenterSvc;
        _projectSvc  = projectSvc;
        _prSvc       = prSvc;
        _poSvc       = poSvc;
        _transferSvc = transferSvc;
        _lineSvc     = lineSvc;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var versions   = (await _versionSvc.GetAllAsync(ct)).ToList();
        var costCenters = (await _costCenterSvc.GetAllAsync(ct)).ToList();
        var projects   = (await _projectSvc.GetAllAsync(ct)).ToList();
        var prs        = (await _prSvc.GetAllAsync(ct)).ToList();
        var pos        = (await _poSvc.GetAllAsync(ct)).ToList();
        var transfers  = (await _transferSvc.GetAllAsync(ct)).ToList();
        var lines      = (await _lineSvc.GetByFilterAsync(null, null, null, ct)).ToList();

        var vm = new DashboardViewModel
        {
            BudgetVersionCount  = versions.Count,
            CostCenterCount     = costCenters.Count,
            ProjectCount        = projects.Count,
            PurchaseRequestCount = prs.Count,
            PurchaseOrderCount  = pos.Count,
            BudgetTransferCount = transfers.Count,
            TotalPlanned        = lines.Sum(l => l.PlannedAmount),
            TotalApproved       = lines.Sum(l => l.ApprovedAmount),
            TotalCommitted      = lines.Sum(l => l.CommittedAmount),
        };
        return View(vm);
    }

    public IActionResult Error() => View();
}
