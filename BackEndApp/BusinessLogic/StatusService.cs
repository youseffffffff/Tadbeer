using BackEndApp.DataAccess;
using BackEndApp.Models.Statuses;

namespace BackEndApp.BusinessLogic;

public class StatusService
{
    private readonly StatusesDataAccess _statusesDataAccess;

    public StatusService(StatusesDataAccess statusesDataAccess)
    {
        _statusesDataAccess = statusesDataAccess;
    }

    public async Task<List<Status>> GetAllAsync()
    {
        return await _statusesDataAccess.GetAllAsync();
    }

    public async Task<Status?> GetByIdAsync(int id)
    {
        return await _statusesDataAccess.GetByIdAsync(id);
    }
}
