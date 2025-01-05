using SharedModels.Models;

namespace DatabaseService;

public class DatabaseService : IDatabaseService
{
    public Task CreateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<User?> FindUserByUsernameAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserPasswordAsync(string username, string newPassword)
    {
        throw new NotImplementedException();
    }
}
