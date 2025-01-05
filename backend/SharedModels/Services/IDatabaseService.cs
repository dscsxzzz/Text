using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedModels.Models;

namespace DatabaseService;

public interface IDatabaseService
{
    Task<User?> FindUserByUsernameAsync(string username);
    Task CreateUserAsync(User user);
    Task UpdateUserPasswordAsync(string username, string newPassword);
    Task DeleteUserAsync(Guid userId);

}
