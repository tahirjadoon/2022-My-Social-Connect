
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;

public class UsersBusinessLogic : IUsersBusinessLogic
{
    private readonly IUsersRepository _usersRepo;

    public UsersBusinessLogic(IUsersRepository usersRepo)
    {
        _usersRepo = usersRepo;
    }

    public async Task<IEnumerable<AppUser>> GetUsers()
    {
        var users = await _usersRepo.GetUsers();
        return users;
    }

    public async Task<AppUser> GetUser(int id)
    {
        var user = await _usersRepo.GetUser(id);
        return user;
    }
    
}