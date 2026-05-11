using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITBudgeting.Web.ViewModels;

public class CreatePurchaseOrderViewModel
{
    [Required]
    public Guid PurchaseRequestId { get; set; }

    [Required, Range(0.01, double.MaxValue)]
    public decimal AmountApproved { get; set; }

    public IEnumerable<SelectListItem> PurchaseRequests { get; set; } = Enumerable.Empty<SelectListItem>();
}

public class RecordUsageViewModel
{
    [Required, Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}
