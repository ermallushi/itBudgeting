using ITBudgeting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/audit")]
[Authorize(Roles = "FinanceController,Admin")]
public class AuditController : ControllerBase
{
    private readonly AuditService _service;

    public AuditController(AuditService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAudit(
        [FromQuery] string entityType,
        [FromQuery] Guid entityId,
        CancellationToken ct)
        => Ok(await _service.GetByEntityAsync(entityType, entityId, ct));
}
