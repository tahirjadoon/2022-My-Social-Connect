using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;
public interface IUsersRepository
{
    Task<IEnumerable<AppUser>> GetUsers();

    Task<AppUser> GetUser(int id);

    Task<AppUser> GetUser(string userName);

    Task<AppUser> Register(AppUser user);

    Task<bool> UserExists(string userName);
}