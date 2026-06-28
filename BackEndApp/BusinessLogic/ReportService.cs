using BackEndApp.DataAccess;
using BackEndApp.Models.Reports;

namespace BackEndApp.BusinessLogic;

public class ReportService
{
    private readonly ReportsDataAccess _reportsDataAccess;
    private readonly BudgetAllocationsDataAccess _budgetAllocationsDataAccess;

    public ReportService(
        ReportsDataAccess reportsDataAccess,
        BudgetAllocationsDataAccess budgetAllocationsDataAccess)
    {
        _reportsDataAccess = reportsDataAccess;
        _budgetAllocationsDataAccess = budgetAllocationsDataAccess;
    }

    public async Task<List<Report>> GetAllAsync()
    {
        return await _reportsDataAccess.GetAllAsync();
    }

    public async Task<Report?> GetByIdAsync(Guid id)
    {
        return await _reportsDataAccess.GetByIdAsync(id);
    }

    public async Task<Report?> AddAsync(Report report)
    {
        if (!PrepareAndValidate(report))
        {
            return null;
        }

        var allocation = await _budgetAllocationsDataAccess.GetByIdAsync(report.AllocationId);

        if (allocation is null
            || await _reportsDataAccess.ExistsByAllocationIdAsync(report.AllocationId))
        {
            return null;
        }

        return await _reportsDataAccess.AddAsync(report);
    }

    public async Task<bool> UpdateAsync(Report report)
    {
        var existingReport = await _reportsDataAccess.GetByIdAsync(report.ReportId);

        if (existingReport is null || !PrepareAndValidate(report))
        {
            return false;
        }

        report.AllocationId = existingReport.AllocationId;
        report.GeneratedAt = existingReport.GeneratedAt;

        return await _reportsDataAccess.UpdateAsync(report);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _reportsDataAccess.DeleteAsync(id);
    }

    private static bool PrepareAndValidate(Report report)
    {
        if (string.IsNullOrWhiteSpace(report.ReportDataJson))
        {
            return false;
        }

        report.ReportDataJson = report.ReportDataJson.Trim();

        return true;
    }
}
