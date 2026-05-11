using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/purchase-orders")]
[Authorize]
public class PurchaseOrdersController : ControllerBase
{
    private readonly PurchaseOrderService _service;

    public PurchaseOrdersController(PurchaseOrderService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, ct);
            return NoContent();
        }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/record-usage")]
    [Authorize(Roles = "BudgetUser,Manager,FinanceController,Admin")]
    public async Task<IActionResult> RecordUsage(Guid id, [FromBody] RecordUsageDto dto, CancellationToken ct)
    {
        try { return Ok(await _service.RecordUsageAsync(id, dto, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
