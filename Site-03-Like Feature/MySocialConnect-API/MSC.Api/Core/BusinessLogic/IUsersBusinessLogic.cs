using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;

public interface IUsersBusinessLogic
{
    //it is returning pageList<userDto>
    //Task<IEnumerable<UserDto>> GetUsersAsync();
    Task<PageList<UserDto>> GetUsersAsync(UserParams userParams);
    Task<UserDto> GetUserByGuidAsync(Guid id);
    Task<UserDto> GetUserAsync(int id);
    Task<UserDto> GetUserAsync(string name);

    /// <summary>
    /// used by the LogUserAcitivty Action Filter
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task LogUserActivityAsync(int userId);

    Task<UserTokenDto> RegisterAsync(UserRegisterDto registerUser);
    Task<UserTokenDto> LoginAsync(LoginDto login);

    Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto, UserClaimGetDto claims);

    Task<PhotoDto> AddPhoto(IFormFile file, UserClaimGetDto claims);
    Task<bool> SetPhotoMain(int photoId, UserClaimGetDto claims);
    Task<BusinessResponse> DeletePhoto(int photoId, UserClaimGetDto claims);
}