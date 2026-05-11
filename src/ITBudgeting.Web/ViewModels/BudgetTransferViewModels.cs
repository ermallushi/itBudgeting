using System.ComponentModel.DataAnnotations;
using ITBudgeting.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.ViewModels;

public class CreateBudgetTransferViewModel
{
    [Required]
    public Guid VersionId { get; set; }

    [Required]
    public Guid FromProjectId { get; set; }

    [Required]
    public Guid ToProjectId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Required]
    public BudgetCategory Category { get; set; }

    [Required, RegularExpression(@"^\d{4}-(0[1-9]|1[0-2])$", ErrorMessage = "Period must be YYYY-MM")]
    public string Period { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string ApprovedBy { get; set; } = string.Empty;

    public IEnumerable<SelectListItem> Projects { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> Versions { get; set; } = Enumerable.Empty<SelectListItem>();
}
