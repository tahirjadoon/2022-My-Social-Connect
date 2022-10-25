using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.ExceptionsCustom;
using MSC.Api.Core.Repositories;
using MSC.Api.Core.Services;

namespace MSC.Api.Core.BusinessLogic;

public class UsersBusinessLogic : IUsersBusinessLogic
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IUsersRepository _usersRepo;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;
    private readonly IMapper _mapper;

    public UsersBusinessLogic(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        RoleManager<AppRole> roleManager,
        IUsersRepository usersRepo,
        ITokenService tokenService,
        IPhotoService photoService,
        IMapper mapper)
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _usersRepo = usersRepo;
        _photoService = photoService;
        _mapper = mapper;
    }

    public async Task<PageList<UserDto>> GetUsersAsync(UserParams userParams)
    {
        var users = await _usersRepo.GetUsersAsync(userParams);
        if (users == null || !users.Any()) return null;
        return users;
    }

    public async Task<UserDto> GetUserByGuidAsync(Guid id)
    {
        var user = await _usersRepo.GetUserByGuidAsync(id);
        if (user == null) return null;

        return user;
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        var user = await _usersRepo.GetUserAsync(id);
        if (user == null) return null;

        return user;
    }

    public async Task<UserDto> GetUserAsync(string name)
    {
        var user = await _usersRepo.GetUserAsync(name);
        if (user == null) return null;

        return user;
    }

    public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto, UserClaimGetDto claims)
    {
        var user = await _usersRepo.GetAppUserAsync(claims.UserId);
        if (user == null || user.GuId != claims.Guid)
            return false;

        //data from the userUpdateDto will be moved to user while the rest of the properties will be kept as is
        var updates = _mapper.Map(userUpdateDto, user);

        //issue update but it will not save
        _usersRepo.Update(updates);

        //save update
        if (await _usersRepo.SaveAllAsync())
            return true;

        return false;
    }

    /// <summary>
    /// used by the LogUserAcitivty Action Filter
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task LogUserActivityAsync(int userId)
    {
        if (userId <= 0) return;

        //app user 
        var user = await _usersRepo.GetAppUserAsync(userId);
        if (user == null) return;

        //update the last active date 
        user.LastActive = DateTime.Now;

        //update 
        await _usersRepo.SaveAllAsync();
    }

    public async Task<UserTokenDto> RegisterAsync(UserRegisterDto registerUser)
    {
        if (registerUser == null)
            throw new ValidationException("Invalid user"); //exception middleware

        var user = await RegisterUser(registerUser);
        if (user == null || user.Id <= 0)
            throw new ValidationException("Unable to create registration"); //exception middleware

        var userToken = _mapper.Map<UserTokenDto>(user);
        userToken.Token = await _tokenService.CreateToken(user);

        return userToken;
    }

    public async Task<UserTokenDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing"); //exception middleware

        var user = await _userManager.Users
                                    .Include(p => p.Photos)
                                    .SingleOrDefaultAsync(x => x.UserName == login.UserName.ToLowerInvariant());
        if (user == null)
            throw new UnauthorizedAccessException("Either username or password is wrong"); //exception middleware

        var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Either username or password is wrong"); //exception middleware

        //build and return user token
        var userToken = _mapper.Map<UserTokenDto>(user);
        userToken.Token = await _tokenService.CreateToken(user);

        return userToken;
    }

    private async Task<AppUser> RegisterUser(UserRegisterDto registerUser)
    {
        if (registerUser == null || string.IsNullOrWhiteSpace(registerUser.UserName) || string.IsNullOrWhiteSpace(registerUser.Password))
            throw new ValidationException("User info missing"); //exception middleware

        //check user not already taken
        var isUser = await _usersRepo.UserExistsAsync(registerUser.UserName);
        if (isUser)
            throw new ValidationException("Username already taken"); //exception middleware

        //convert to AppUser to register
        var user = _mapper.Map<AppUser>(registerUser);

        //using userManager register create the user 
        var result = await _userManager.CreateAsync(user, registerUser.Password);
        if (!result.Succeeded)
            throw new DataFailException(result.Errors.ToString());

        //add the user to the member role as well 
        var roleResult = await _userManager.AddToRoleAsync(user, SiteIdentityConstants.Role_Member);
        if (!roleResult.Succeeded)
            throw new DataFailException(roleResult.Errors.ToString());

        var returnUser = await _usersRepo.GetAppUserAsync(user.UserName);
        if (returnUser == null)
            throw new DataFailException("Something went wrong. No user found!");

        return returnUser;
    }

    public async Task<PhotoDto> AddPhoto(IFormFile file, UserClaimGetDto claims)
    {
        //get app user with photos
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserId, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found"); //exception middleware

        var result = await _photoService.AddPhotoAsync(file);

        //error
        if (result.Error != null)
            throw new DataFailException(result.Error?.Message ?? "Photo updload error"); //exception middleware

        //success, build photo entity and save
        var photo = new Photo
        {
            Url = result.SecureUrl.AbsoluteUri, //set photo url
            PublicId = result.PublicId, //set public id
            IsMain = appUser.Photos == null || !appUser.Photos.Any() //mark it active when no other photos are available
        };

        //add the photo. Photos is an abstract method so cannot be null
        appUser.Photos.Add(photo);

        if (await _usersRepo.SaveAllAsync())
        {
            return _mapper.Map<PhotoDto>(photo);
        }

        return null;
    }

    public async Task<bool> SetPhotoMain(int photoId, UserClaimGetDto claims)
    {
        //get app user with photos
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserId, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found"); //exception middleware

        var photo = appUser.Photos?.FirstOrDefault(x => x.Id == photoId);
        if (photo == null)
            return false;

        if (photo.IsMain)
            throw new DataFailException("This is already your main photo"); //exception middleware

        var currentMain = appUser.Photos.FirstOrDefault(x => x.IsMain == true);
        if (currentMain != null)
            currentMain.IsMain = false;

        photo.IsMain = true;

        if (await _usersRepo.SaveAllAsync())
            return true;

        return false;
    }

    public async Task<BusinessResponse> DeletePhoto(int photoId, UserClaimGetDto claims)
    {
        //get app user with photos
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserId, includePhotos: true);
        if (appUser == null)
            throw new UnauthorizedAccessException("User not found"); //exception middleware

        var response = new BusinessResponse();

        var photo = appUser.Photos?.FirstOrDefault(x => x.Id == photoId);
        if (photo == null)
        {
            response.HttpStatusCode = HttpStatusCode.NotFound;
            response.Message = "Photo not found";
            return response;
        }

        if (photo.IsMain)
        {
            response.HttpStatusCode = HttpStatusCode.BadRequest;
            response.Message = "You cannot delete your main photo";
            return response;
        }

        //delete from cloudinary
        if (photo.PublicId != null)
        {
            var result = await _photoService.DeletePhotoAync(photo.PublicId);
            if (result.Error != null)
            {
                response.HttpStatusCode = HttpStatusCode.BadRequest;
                response.Message = result.Error.Message;
                return response;
            }
        }

        //remove from the database as well 
        appUser.Photos.Remove(photo);

        if (await _usersRepo.SaveAllAsync())
        {
            response.HttpStatusCode = HttpStatusCode.OK;
            return response;
        }

        //it is an error then 
        response.HttpStatusCode = HttpStatusCode.BadRequest;
        response.Message = "Unable to delete photo";
        return response;
    }

    public async Task<IEnumerable<object>> GetUSersWithRoles()
    {
        //get the users, include UserRoles and then Role
        //return an annonamous object
        var users = await _userManager.Users
                                    .Include(r => r.UserRoles)
                                    .ThenInclude(r => r.Role)
                                    .OrderBy(u => u.UserName)
                                    .Select(u => new
                                    {
                                        u.Id,
                                        UserName = u.UserName,
                                        DisplayName = u.DisplayName,
                                        GuId = u.GuId,
                                        Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                                    })
                                    .ToListAsync()
                                    ;
        return users;
    }

    public async Task<BusinessResponse> EditRolesForUser(int adminUSerId, Guid userToUpdate, IEnumerable<string> roles)
    {
        //check user
        var user = await _userManager.Users.SingleOrDefaultAsync(x => x.GuId == userToUpdate);
        if (user == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "User not found to update");

        //check roles to update
        if (roles == null || !roles.Any())
            return new BusinessResponse(HttpStatusCode.BadRequest, "No roles passed to update");

        //get the siteRoles
        var siteRoles = await _roleManager.Roles.Select(x => x.Name).ToListAsync();

        //check roles to update are in the site roles
        var updateRolesNotInSiteRoles = roles.Where(x => !siteRoles.Any(y => y == x)).ToList();
        if (updateRolesNotInSiteRoles != null && updateRolesNotInSiteRoles.Any())
            return new BusinessResponse(HttpStatusCode.BadRequest, $"Passed role(s) not in list {string.Join(", ", updateRolesNotInSiteRoles)}");

        //current user roles 
        var userRoles = await _userManager.GetRolesAsync(user);

        //add the new roles that are not in current roles
        var result = await _userManager.AddToRolesAsync(user, roles.Except(userRoles));
        if (!result.Succeeded)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Failed to add to roles");

        //remove the roles as well since the user may have removes some. Above is only adding new ones 
        var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(roles));
        if (!removeResult.Succeeded)
            return new BusinessResponse(HttpStatusCode.BadRequest, "Failed to remove roles");

        //pick new roles
        var currentRoles = await _userManager.GetRolesAsync(user);

        return new BusinessResponse(HttpStatusCode.OK, "Roles updated successfully", currentRoles);
    }
}