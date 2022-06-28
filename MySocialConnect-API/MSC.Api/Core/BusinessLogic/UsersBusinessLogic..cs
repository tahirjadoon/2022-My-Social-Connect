
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

    public async Task<IEnumerable<UserDto>> GetUsers()
    {
        var users = await _usersRepo.GetUsers();
        if(users == null || !users.Any())
            return null;

        var userDto = users.Select(x => new UserDto { Id = x.Id, UserName = x.UserName }).ToList();
        return userDto;
    }

    public async Task<UserDto> GetUser(int id)
    {
        var user = await _usersRepo.GetUser(id);
        if(user == null)
            return null;

        var userDto = new UserDto { Id = user.Id, UserName = user.UserName };
        return userDto;
    }

    public async Task<UserTokenDto> Register(UserRegisterDto registerUser)
    {
        if (registerUser == null)
            throw new ValidationException("Invalid user");

        var user = await RegisterUser(registerUser);
        if(user == null || user.Id <= 0)
            throw new ValidationException("Unable to create registration");

        var userToken = new UserTokenDto
        {
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        return userToken;
    }

    public async Task<UserTokenDto> Login(LoginDto login)
    {
        if (login == null)
            throw new ValidationException("Login info missing");

        var user = await _usersRepo.GetUser(login.UserName);
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
        var isUser = await _usersRepo.UserExists(registerUser.UserName);
        if (isUser)
            throw new ValidationException("Username already taken");

        //has the password. it will give back has and the salt
        var hashKey = registerUser.Password.ComputeHashHmacSha512();
        if (hashKey == null)
            throw new ValidationException("Unable to handle provided password");

        //convert to AppUser to register
        var user = new AppUser { UserName = registerUser.UserName, PasswordHash = hashKey.Hash, PasswordSalt = hashKey.Salt };

        user = await _usersRepo.Register(user);

        return user;
    }
}