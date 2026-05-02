using System.ComponentModel.DataAnnotations;
using ITBudgeting.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.ViewModels;

public class CreateProjectViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid CostCenterId { get; set; }

    [Required]
    public BudgetCategory Category { get; set; }

    public IEnumerable<SelectListItem> CostCenters { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class EditProjectViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public BudgetCategory Category { get; set; }

    public bool IsActive { get; set; } = true;
}
