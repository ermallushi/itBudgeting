using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/budget-lines")]
[Authorize]
public class BudgetLinesController : ControllerBase
{
    private readonly BudgetLineService _service;

    public BudgetLinesController(BudgetLineService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? versionId,
        [FromQuery] Guid? costCenterId,
        [FromQuery] Guid? projectId,
        CancellationToken ct)
        => Ok(await _service.GetByFilterAsync(versionId, costCenterId, projectId, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _service.GetByIdAsync(id, ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBudgetLineDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetLineDto dto, CancellationToken ct)
    {
        try { return Ok(await _service.UpdateAsync(id, dto, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
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
}
