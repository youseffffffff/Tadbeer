namespace BackEndApp.Models.ExpenseCategories;

public class ExpenseCategory
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public string? IconName { get; set; }

    public string? ColorHex { get; set; }

    public DateTime? CreatedAt { get; set; }

    public ExpenseCategoryDto ToDto()
    {
        return new ExpenseCategoryDto
        {
            CategoryId = CategoryId,
            CategoryName = CategoryName,
            IconName = IconName,
            ColorHex = ColorHex,
            CreatedAt = CreatedAt
        };
    }
}
