namespace BackEndApp.Models.BudgetAllocations;

public class BudgetAllocation
{
    public Guid AllocationId { get; set; }

    public Guid PeriodId { get; set; }

    public Guid UserId { get; set; }

    public decimal AllocatedAmount { get; set; }

    public bool? CanOverspend { get; set; }

    public bool? ApprovalRequired { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public BudgetAllocationDto ToDto()
    {
        return new BudgetAllocationDto
        {
            AllocationId = AllocationId,
            PeriodId = PeriodId,
            UserId = UserId,
            AllocatedAmount = AllocatedAmount,
            CanOverspend = CanOverspend,
            ApprovalRequired = ApprovalRequired,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}
