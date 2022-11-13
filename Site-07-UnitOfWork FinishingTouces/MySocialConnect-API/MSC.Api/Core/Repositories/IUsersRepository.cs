using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;
public interface IUsersRepository
{
    void Update(AppUser user);
    //Task<bool> SaveAllAsync();
    //Rather than returning list of userDto, will return pageList<userDto>
    //Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<PageList<UserDto>> GetUsersAsync(UserParams userParams);
    Task<UserDto> GetUserByGuidAsync(Guid id);
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> GetUserAsync(string userName);
    Task<AppUser> GetAppUserAsync(string userName, bool includePhotos = false);
    Task<AppUser> GetAppUserAsync(int id, bool includePhotos = false);
    void Register(AppUser appUser);
    Task<bool> UserExistsAsync(string userName);
}