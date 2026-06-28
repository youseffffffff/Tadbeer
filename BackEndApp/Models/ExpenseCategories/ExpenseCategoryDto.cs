namespace BackEndApp.Models.ExpenseCategories;

public class ExpenseCategoryDto
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? IconName { get; set; }

    public string? ColorHex { get; set; }

    public DateTime? CreatedAt { get; set; }

    public ExpenseCategory ToModel()
    {
        return new ExpenseCategory
        {
            CategoryName = CategoryName,
            IconName = IconName,
            ColorHex = ColorHex
        };
    }
}
