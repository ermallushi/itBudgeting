using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.ViewModels;

public class CreatePurchaseRequestViewModel
{
    [Required]
    public Guid ProjectId { get; set; }

    [Required]
    public Guid CostCenterId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Url]
    public string? SharePointUrl { get; set; }

    public IEnumerable<SelectListItem> Projects { get; set; } = Enumerable.Empty<SelectListItem>();
    public IEnumerable<SelectListItem> CostCenters { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class EditPurchaseRequestViewModel
{
    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }

    [Url]
    public string? SharePointUrl { get; set; }
}
