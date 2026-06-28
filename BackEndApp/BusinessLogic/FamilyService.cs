using BackEndApp.DataAccess;
using BackEndApp.Models.Families;

namespace BackEndApp.BusinessLogic;

public class FamilyService
{
    private readonly FamiliesDataAccess _familiesDataAccess;

    public FamilyService(FamiliesDataAccess familiesDataAccess)
    {
        _familiesDataAccess = familiesDataAccess;
    }

    public async Task<List<Family>> GetAllAsync()
    {
        return await _familiesDataAccess.GetAllAsync();
    }

    public async Task<Family?> GetByIdAsync(Guid id)
    {
        return await _familiesDataAccess.GetByIdAsync(id);
    }

    public async Task<Family?> AddAsync(Family family)
    {
        return await _familiesDataAccess.AddAsync(family);
    }

    public async Task<bool> UpdateAsync(Family family)
    {
        return await _familiesDataAccess.UpdateAsync(family);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        return await _familiesDataAccess.DeleteAsync(id);
    }
}
