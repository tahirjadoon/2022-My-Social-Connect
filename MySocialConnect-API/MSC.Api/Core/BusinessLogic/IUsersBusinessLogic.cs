using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;

public interface IUsersBusinessLogic
{
    Task<IEnumerable<UserDto>> GetUsers();

    Task<UserDto> GetUser(int id);

    Task<UserTokenDto> Register(UserRegisterDto registerUser);

    Task<UserTokenDto> Login(LoginDto login);

}