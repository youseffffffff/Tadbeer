namespace BackEndApp.Models.BudgetPeriods;

public class BudgetPeriodDto
{
    public Guid PeriodId { get; set; }

    public Guid BudgetId { get; set; }

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public decimal ActualBudgetAmount { get; set; }

    public BudgetPeriod ToModel()
    {
        return new BudgetPeriod
        {
            BudgetId = BudgetId,
            PeriodStart = PeriodStart,
            PeriodEnd = PeriodEnd,
            ActualBudgetAmount = ActualBudgetAmount
        };
    }
}
