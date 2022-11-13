using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;

namespace MSC.Api.Controllers;
public class AccountController : BaseApiController
{
    private readonly IUsersBusinessLogic _userBl;

    public AccountController(IUsersBusinessLogic userBl)
    {
        _userBl = userBl;
    }

    /// <summary>
    /// Register users. Required attribute applied to the dto properties
    /// </summary>
    /// <param name="registerUser"></param>
    /// <returns></returns>
    [HttpPost("register")]
    public async Task<ActionResult<UserTokenDto>> Register([FromBody] UserRegisterDto registerUser)
    {
        var user = await _userBl.RegisterAsync(registerUser);

        if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Token))
            return BadRequest("Unable to create registration");

        return Ok(user);
    }

    /// <summary>
    /// Login user. Required attribute applied to the dto properties
    /// </summary>
    /// <param name="loging"></param>
    /// <returns></returns>
    [HttpPost("login")]
    public async Task<ActionResult<UserTokenDto>> Login([FromBody] LoginDto login)
    {
        var user = await _userBl.LoginAsync(login);

        if (user == null || string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Token))
            return Unauthorized("Unable to login user");

        return Ok(user);
    }
}