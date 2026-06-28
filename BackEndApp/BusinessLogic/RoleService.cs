using BackEndApp.DataAccess;
using BackEndApp.Models.Roles;

namespace BackEndApp.BusinessLogic;

public class RoleService
{
    private readonly RolesDataAccess _rolesDataAccess;

    public RoleService(RolesDataAccess rolesDataAccess)
    {
        _rolesDataAccess = rolesDataAccess;
    }

    public async Task<List<Role>> GetAllAsync()
    {
        return await _rolesDataAccess.GetAllAsync();
    }

    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _rolesDataAccess.GetByIdAsync(id);
    }
}
