using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;

public interface IUsersBusinessLogic
{
    Task<IEnumerable<AppUser>> GetUsers();

    Task<AppUser> GetUser(int id);
}