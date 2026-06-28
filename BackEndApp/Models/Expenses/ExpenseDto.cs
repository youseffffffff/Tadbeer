namespace BackEndApp.Models.Expenses;

public class ExpenseDto
{
    public Guid ExpenseId { get; set; }

    public Guid AllocationId { get; set; }

    public int CategoryId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public DateTime ExpenseDate { get; set; }

    public byte? SourceType { get; set; }

    public bool? IsFlagged { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public Expense ToModel()
    {
        return new Expense
        {
            AllocationId = AllocationId,
            CategoryId = CategoryId,
            Amount = Amount,
            Description = Description,
            ExpenseDate = ExpenseDate,
            SourceType = SourceType,
            IsFlagged = IsFlagged
        };
    }
}
