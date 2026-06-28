using BackEndApp.DataAccess;
using BackEndApp.Models.Users;

namespace BackEndApp.BusinessLogic;

public class UserService
{
    private readonly UsersDataAccess _usersDataAccess;

    public UserService(UsersDataAccess usersDataAccess)
    {
        _usersDataAccess = usersDataAccess;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _usersDataAccess.GetAllAsync();
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _usersDataAccess.GetByIdAsync(userId);
    }

    public async Task<User?> AddAsync(User user)
    {
        return await _usersDataAccess.AddAsync(user);
    }

    public async Task<bool> UpdateAsync(User user)
    {
        return await _usersDataAccess.UpdateAsync(user);
    }

    public async Task<bool> DeleteAsync(Guid userId)
    {
        return await _usersDataAccess.DeleteAsync(userId);
    }
}
