using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Entities;

namespace MSC.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUsersBusinessLogic _usersBl;

    public UsersController(IUsersBusinessLogic userBl)
    {
        _usersBl = userBl;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
    {
        var users = await _usersBl.GetUsers();
        if(users == null || !users.Any())
        {
            return NotFound("No users found!");
        }

        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppUser>> GetUser(int id)
    {
        var user = await _usersBl.GetUser(id);
        if(user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return Ok(user);
    }
}