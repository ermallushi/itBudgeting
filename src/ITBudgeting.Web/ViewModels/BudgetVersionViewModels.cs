using System.ComponentModel.DataAnnotations;
using ITBudgeting.Application.DTOs;
using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Web.ViewModels;

public class CreateBudgetVersionViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, Range(2000, 2100)]
    public int Year { get; set; } = DateTime.Now.Year;

    [Required]
    public BudgetVersionType Type { get; set; } = BudgetVersionType.Draft;

    public Guid? ParentVersionId { get; set; }
}

public class EditBudgetVersionViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}

public class CloneRevisionViewModel
{
    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public Guid ParentId { get; set; }
    public bool[] OpenMonths { get; set; } = new bool[12];
}
