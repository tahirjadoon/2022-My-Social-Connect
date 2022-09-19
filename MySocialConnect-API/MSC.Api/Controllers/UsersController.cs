using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Controllers;

[Authorize]
public class UsersController : BaseApiController
{
    private readonly IUsersBusinessLogic _usersBl;

    public UsersController(IUsersBusinessLogic userBl)
    {
        _usersBl = userBl;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _usersBl.GetUsersAsync();
        if(users == null || !users.Any())
        {
            return NotFound("No users found!");
        }

        return Ok(users);
    }

    [HttpGet("{guid}/guid")]
    public async Task<ActionResult<UserDto>> GetUser(Guid guid)
    {
        var user = await _usersBl.GetUserByGuidAsync(guid);
        if(user == null)
        {
            return NotFound($"No user found by guid {guid}");
        }

        return Ok(user);
    }

    [HttpGet("{id}/id")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _usersBl.GetUserAsync(id);
        if(user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return Ok(user);
    }

    [HttpGet("{name}/name")]
    public async Task<ActionResult<UserDto>> GetUser(string name)
    {
        var user = await _usersBl.GetUserAsync(name);
        if(user == null)
        {
            return NotFound($"No user found by name {name}");
        }

        return Ok(user);
    }
}