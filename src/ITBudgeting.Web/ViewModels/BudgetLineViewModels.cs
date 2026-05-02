using System.ComponentModel.DataAnnotations;
using ITBudgeting.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.ViewModels;

public class CreateBudgetLineViewModel
{
    [Required]
    public Guid VersionId { get; set; }

    [Required]
    public Guid CostCenterId { get; set; }

    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public BudgetCategory Category { get; set; }

    [Required, RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Period must be YYYY-MM")]
    public string Period { get; set; } = string.Empty;

    [Required, Range(0, double.MaxValue)]
    public decimal PlannedAmount { get; set; }

    public IEnumerable<SelectListItem> CostCenters { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Projects { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class EditBudgetLineViewModel
{
    [Required, Range(0, double.MaxValue)]
    public decimal PlannedAmount { get; set; }

    [Required, Range(0, double.MaxValue)]
    public decimal ApprovedAmount { get; set; }

    [Required, Range(0, double.MaxValue)]
    public decimal ActualAmount { get; set; }
}
