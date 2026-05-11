using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/budget-periods")]
[Authorize]
public class BudgetPeriodsController : ControllerBase
{
    private readonly BudgetPeriodService _service;

    public BudgetPeriodsController(BudgetPeriodService service)
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
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Create([FromBody] CreateBudgetPeriodDto dto, CancellationToken ct)
    {
        var result = await _service.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "FinanceController,Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBudgetPeriodDto dto, CancellationToken ct)
    {
        try { return Ok(await _service.UpdateAsync(id, dto, ct)); }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
