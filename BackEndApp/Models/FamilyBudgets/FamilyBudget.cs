namespace BackEndApp.Models.FamilyBudgets;

public class FamilyBudget
{
    public Guid BudgetId { get; set; }

    public Guid FamilyId { get; set; }

    public byte BudgetCycleType { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public FamilyBudgetDto ToDto()
    {
        return new FamilyBudgetDto
        {
            BudgetId = BudgetId,
            FamilyId = FamilyId,
            BudgetCycleType = BudgetCycleType,
            IsActive = IsActive,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}
