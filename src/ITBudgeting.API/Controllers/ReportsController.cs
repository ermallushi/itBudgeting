using ITBudgeting.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportService _service;

    public ReportsController(ReportService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] Guid versionId, CancellationToken ct)
        => Ok(await _service.GetSummaryAsync(versionId, ct));

    [HttpGet("variance")]
    public async Task<IActionResult> GetVariance([FromQuery] Guid versionId, CancellationToken ct)
        => Ok(await _service.GetVarianceAsync(versionId, ct));
}
