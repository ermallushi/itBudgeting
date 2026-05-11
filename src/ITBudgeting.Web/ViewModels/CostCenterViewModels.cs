using System.ComponentModel.DataAnnotations;

namespace ITBudgeting.Web.ViewModels;

public class CreateCostCenterViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;
}

public class EditCostCenterViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Department { get; set; } = string.Empty;
}
