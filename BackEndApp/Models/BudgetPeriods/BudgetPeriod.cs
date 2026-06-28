namespace BackEndApp.Models.BudgetPeriods;

public class BudgetPeriod
{
    public Guid PeriodId { get; set; }

    public Guid BudgetId { get; set; }

    public DateTime PeriodStart { get; set; }

    public DateTime PeriodEnd { get; set; }

    public decimal ActualBudgetAmount { get; set; }

    public BudgetPeriodDto ToDto()
    {
        return new BudgetPeriodDto
        {
            PeriodId = PeriodId,
            BudgetId = BudgetId,
            PeriodStart = PeriodStart,
            PeriodEnd = PeriodEnd,
            ActualBudgetAmount = ActualBudgetAmount
        };
    }
}
