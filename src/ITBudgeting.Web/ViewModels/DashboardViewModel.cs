namespace ITBudgeting.Web.ViewModels;

public class DashboardViewModel
{
    public int BudgetVersionCount { get; set; }
    public int CostCenterCount { get; set; }
    public int ProjectCount { get; set; }
    public int PurchaseRequestCount { get; set; }
    public int PurchaseOrderCount { get; set; }
    public int BudgetTransferCount { get; set; }
    public decimal TotalPlanned { get; set; }
    public decimal TotalApproved { get; set; }
    public decimal TotalCommitted { get; set; }
}
