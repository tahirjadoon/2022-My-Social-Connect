using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;

public interface IUsersBusinessLogic
{
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> GetUserAsync(string name);
    Task<UserTokenDto> RegisterAsync(UserRegisterDto registerUser);
    Task<UserTokenDto> LoginAsync(LoginDto login);
}