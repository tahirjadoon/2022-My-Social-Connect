using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Controllers;

public class UsersController : BaseApiController
{
    private readonly IUsersBusinessLogic _usersBl;

    public UsersController(IUsersBusinessLogic userBl)
    {
        _usersBl = userBl;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _usersBl.GetUsers();
        if(users == null || !users.Any())
        {
            return NotFound("No users found!");
        }

        return Ok(users);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _usersBl.GetUser(id);
        if(user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return Ok(user);
    }
}