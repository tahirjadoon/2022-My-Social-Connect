using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Controllers;
public class AccountController : BaseApiController
{
    private readonly IUsersBusinessLogic _userBl;

    public AccountController(IUsersBusinessLogic userBl)
    {
        _userBl = userBl;
    }

    /*
    [HttpPost("register")]
    public async Task<ActionResult<AppUser>> Register([FromQuery] string userName, [FromBody] string password)
    {
        try
        {
            var user = await _userBl.Register(userName, password);

            if(user == null || user.Id <= 0)
                return BadRequest("Unable to create registration");

            return Ok(user);
        }
        catch(ValidationException vEx)
        {
            return BadRequest(vEx.Message);
        }
        catch
        {
            throw;
        }
    }
    */

    /// <summary>
    /// Register users. Required attribute applied to the dto properties
    /// </summary>
    /// <param name="registerUser"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserTokenDto>> Register([FromBody] UserRegisterDto registerUser)
    {
        try
        {
            var user = await _userBl.Register(registerUser);

            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Token))
                return BadRequest("Unable to create registration");

            return Ok(user);
        }
        catch(ValidationException vEx)
        {
            return BadRequest(vEx.Message);
        }
        catch
        {
            throw;
        }
    }

    /// <summary>
    /// Login user. Required attribute applied to the dto properties
    /// </summary>
    /// <param name="loging"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserTokenDto>>Login([FromBody] LoginDto login)
    {
         try
        {
            var user = await _userBl.Login(login);

            if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Token))
                return Unauthorized("Unable to login user");

            return Ok(user);
        }
        catch(ValidationException vEx)
        {
            return Unauthorized(vEx.Message);
        }
        catch(UnauthorizedAccessException aEx)
        {
            return Unauthorized(aEx.Message);
        }
        catch
        {
            throw;
        }
    }
}