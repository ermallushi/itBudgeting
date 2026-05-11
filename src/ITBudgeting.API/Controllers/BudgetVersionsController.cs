using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/budget-versions")]
[Authorize]
public class BudgetVersionsController : ControllerBase
{
    private readonly BudgetVersionService _service;

    public BudgetVersionsController(BudgetVersionService service)
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
    public async Task<IActionResult> Create([FromBody] CreateBudgetVersionDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetVersionDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _service.UpdateAsync(id, dto, ct);
            return Ok(result);
        }
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

    [HttpPost("{id:guid}/submit")]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> Submit(Guid id, CancellationToken ct)
    {
        try { return Ok(await _service.SubmitAsync(id, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken ct)
    {
        try { return Ok(await _service.ApproveAsync(id, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/lock")]
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Lock(Guid id, CancellationToken ct)
    {
        try { return Ok(await _service.LockAsync(id, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }

    [HttpPost("{id:guid}/clone-revision")]
    [Authorize(Roles = "Manager,FinanceController,Admin")]
    public async Task<IActionResult> CloneRevision(Guid id, [FromBody] CloneRevisionDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _service.CloneRevisionAsync(id, dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
