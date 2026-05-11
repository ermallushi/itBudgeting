namespace ITBudgeting.Domain.Exceptions;

public class BudgetDomainException : Exception
{
    public BudgetDomainException(string message) : base(message) { }
    public BudgetDomainException(string message, Exception innerException) : base(message, innerException) { }
}
