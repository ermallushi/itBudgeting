using ITBudgeting.Application.DTOs;
using ITBudgeting.Application.Services;
using ITBudgeting.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITBudgeting.API.Controllers;

[ApiController]
[Route("api/budget-transfers")]
[Authorize]
public class BudgetTransfersController : ControllerBase
{
    private readonly BudgetTransferService _service;

    public BudgetTransfersController(BudgetTransferService service)
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
    public async Task<IActionResult> Create([FromBody] CreateBudgetTransferDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }
        catch (BudgetDomainException ex) { return BadRequest(new { error = ex.Message }); }
    }
}
