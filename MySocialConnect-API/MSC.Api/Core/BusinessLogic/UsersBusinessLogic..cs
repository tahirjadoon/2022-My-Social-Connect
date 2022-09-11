using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;
using MSC.Api.Core.Repositories;
using MSC.Api.Core.Services;

namespace MSC.Api.Core.BusinessLogic;

public class UsersBusinessLogic : IUsersBusinessLogic
{
    private readonly IUsersRepository _usersRepo;
    private readonly ITokenService _tokenService;

    public UsersBusinessLogic(IUsersRepository usersRepo, ITokenService tokenService)
    {
        _tokenService = tokenService;
        _usersRepo = usersRepo;
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

    public async Task<UserTokenDto> RegisterAsync(UserRegisterDto registerUser)
    {
        if (registerUser == null)
            throw new ValidationException("Invalid user");

        var user = await RegisterUser(registerUser);
        if (user == null || user.Id <= 0)
            throw new ValidationException("Unable to create registration");

        var userToken = new UserTokenDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        return userToken;
    }

    public async Task<UserTokenDto> LoginAsync(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing");

        var user = await _usersRepo.GetAppUserAsync(login.UserName);
        if (user == null || user.PasswordSalt == null || user.PasswordHash == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //password is hashed in db. Hash login password and check against the DB one
        var hashKeyLogin = login.Password.ComputeHashHmacSha512(user.PasswordSalt);
        if (hashKeyLogin == null)
            throw new UnauthorizedAccessException("Either username or password is wrong");

        //both are byte[]
        if (!hashKeyLogin.Hash.AreEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Either username or password is wrong");

        var userToken = new UserTokenDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        return userToken;
    }

    private async Task<AppUser> RegisterUser(UserRegisterDto registerUser)
    {
        if (registerUser == null || string.IsNullOrWhiteSpace(registerUser.UserName) || string.IsNullOrWhiteSpace(registerUser.Password))
            throw new ValidationException("User info missing");

        //check user not already taken
        var isUser = await _usersRepo.UserExistsAsync(registerUser.UserName);
        if (isUser)
            throw new ValidationException("Username already taken");

        //hash the password. it will give back hash and the salt
        var hashKey = registerUser.Password.ComputeHashHmacSha512();
        if (hashKey == null)
            throw new ValidationException("Unable to handle provided password");

        //convert to AppUser to register
        var user = new AppUser { UserName = registerUser.UserName, PasswordHash = hashKey.Hash, PasswordSalt = hashKey.Salt };

        var isRegister = await _usersRepo.RegisterAsync(user);
        if(!isRegister)
            throw new ValidationException("User not registerd");

        var returnUser = await _usersRepo.GetAppUserAsync(user.UserName);
        if(returnUser == null)
            throw new ValidationException("Something went wrong. No user found!");

        return returnUser;
    }
}