using System.ComponentModel.DataAnnotations;
using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Web.ViewModels;

public class EditBudgetPeriodViewModel
{
    public bool IsOpen { get; set; }

    [Required]
    public PeriodLockType LockType { get; set; }
}
