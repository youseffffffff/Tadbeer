namespace BackEndApp.Models.FamilyBudgets;

public class FamilyBudgetDto
{
    public Guid BudgetId { get; set; }

    public Guid FamilyId { get; set; }

    public byte BudgetCycleType { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public FamilyBudget ToModel()
    {
        return new FamilyBudget
        {
            FamilyId = FamilyId,
            BudgetCycleType = BudgetCycleType,
            IsActive = IsActive
        };
    }
}
