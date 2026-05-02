using ITBudgeting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.Web.Controllers;

[Authorize]
public class AuditController : Controller
{
    private readonly AuditService _svc;

    public AuditController(AuditService svc) => _svc = svc;

    public async Task<IActionResult> Index(string? entityType, Guid? entityId, CancellationToken ct)
    {
        ViewBag.EntityType = entityType;
        ViewBag.EntityId   = entityId;

        if (string.IsNullOrEmpty(entityType) || entityId is null)
            return View(Enumerable.Empty<ITBudgeting.Application.DTOs.BudgetAuditDto>());

        var items = await _svc.GetByEntityAsync(entityType, entityId.Value, ct);
        return View(items);
    }
}
