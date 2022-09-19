using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;
public interface IUsersRepository
{
    void Update(AppUser user);
    Task<bool> SaveAllAsync();
    Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<UserDto> GetUserByGuidAsync(Guid id);
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> GetUserAsync(string userName);
    Task<AppUser> GetAppUserAsync(string userName);
    Task<bool> RegisterAsync(AppUser user);
    Task<bool> UserExistsAsync(string userName);
}