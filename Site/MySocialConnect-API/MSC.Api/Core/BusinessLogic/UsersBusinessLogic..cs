using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;
using MSC.Api.Core.ExceptionsCustom;
using MSC.Api.Core.Extensions;
using MSC.Api.Core.Repositories;
using MSC.Api.Core.Services;

namespace MSC.Api.Core.BusinessLogic;

public class UsersBusinessLogic : IUsersBusinessLogic
{
    private readonly IUsersRepository _usersRepo;
    private readonly ITokenService _tokenService;
    private readonly IPhotoService _photoService;
    private readonly IMapper _mapper;

    public UsersBusinessLogic(IUsersRepository usersRepo, ITokenService tokenService, IPhotoService photoService, IMapper mapper)
    {
        _tokenService = tokenService;
        _usersRepo = usersRepo;
        _photoService = photoService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        var users = await _usersRepo.GetUsersAsync();
        if (users == null || !users.Any()) return null;

        //var userDto = users.Select(x => new UserDto { Id = x.Id, UserName = x.UserName }).ToList();
        //var userDto = _mapper.Map<IEnumerable<UserDto>>(users);
        //return userDto;
        return users;
    }

    public async Task<UserDto> GetUserByGuidAsync(Guid id)
    {
        var user = await _usersRepo.GetUserByGuidAsync(id);
        if (user == null) return null;

        //var userDto = new UserDto { Id = user.Id, UserName = user.UserName };
        //var userDto = _mapper.Map<UserDto>(user);
        //return userDto;
        return user;
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        var user = await _usersRepo.GetUserAsync(id);
        if (user == null) return null;

        //var userDto = new UserDto { Id = user.Id, UserName = user.UserName };
        //var userDto = _mapper.Map<UserDto>(user);
        //return userDto;
        return user;
    }

    public async Task<UserDto> GetUserAsync(string name)
    {
        var user = await _usersRepo.GetUserAsync(name);
        if (user == null) return null;

        //var userDto = new UserDto { Id = user.Id, UserName = user.UserName };
        //var userDto = _mapper.Map<UserDto>(user);
        //return userDto;
        return user;
    }

    public async Task<bool> UpdateUserAsync(UserUpdateDto userUpdateDto, UserClaimGetDto claims)
    {
        var user = await _usersRepo.GetAppUserAsync(claims.UserName);
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

    public async Task<UserTokenDto> RegisterAsync(UserRegisterDto registerUser)
    {
        if (registerUser == null)
            throw new ValidationException("Invalid user"); //exception middleware

        var user = await RegisterUser(registerUser);
        if (user == null || user.Id <= 0)
            throw new ValidationException("Unable to create registration"); //exception middleware

        var userToken = _mapper.Map<UserTokenDto>(user);
        userToken.Token = _tokenService.CreateToken(user);

        return userToken;
    }

    public async Task<UserTokenDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing"); //exception middleware

        var user = await _usersRepo.GetAppUserAsync(login.UserName, includePhotos: true);
        if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
            throw new UnauthorizedAccessException("Either username or password is wrong"); //exception middleware

        //password is hashed in db. Hash login password and check against the DB one
        var hashKeyLogin = login.Password.ComputeHashHmacSha512(user.PasswordSalt);
        if (hashKeyLogin == null)
            throw new UnauthorizedAccessException("Either username or password is wrong"); //exception middleware

        //both are byte[]
        if (!hashKeyLogin.Hash.AreEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Either username or password is wrong"); //exception middleware

        //build and return user token
        var userToken = _mapper.Map<UserTokenDto>(user);
        userToken.Token = _tokenService.CreateToken(user);

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

        //hash the password. it will give back hash and the salt
        var hashKey = registerUser.Password.ComputeHashHmacSha512();
        if (hashKey == null)
            throw new ValidationException("Unable to handle provided password"); //exception middleware

        //convert to AppUser to register
        var user = _mapper.Map<AppUser>(registerUser);
        user.PasswordHash = hashKey.Hash;
        user.PasswordSalt = hashKey.Salt;

        var isRegister = await _usersRepo.RegisterAsync(user);
        if (!isRegister)
            throw new DataFailException("User not registerd");

        var returnUser = await _usersRepo.GetAppUserAsync(user.UserName);
        if (returnUser == null)
            throw new DataFailException("Something went wrong. No user found!");

        return returnUser;
    }

    public async Task<PhotoDto> AddPhoto(IFormFile file, UserClaimGetDto claims)
    {
        //get app user with photos
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserName, includePhotos: true);
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
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserName, includePhotos: true);
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
        var appUser = await _usersRepo.GetAppUserAsync(claims.UserName, includePhotos: true);
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
}