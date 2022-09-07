using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;

namespace MSC.Api.Controllers;
public class BuggyController : BaseApiController
{
    private readonly IUsersBusinessLogic _userBl;
    public BuggyController(IUsersBusinessLogic userBl)
    {
        _userBl = userBl;
    }

    [Authorize]
    [HttpGet("auth")]
    public async Task<ActionResult<string>> GetSecret()
    {
        //dummy call
        var thing = await _userBl.GetUser(-1);
        return Ok("secret text");
    }

    [HttpGet("not-found")]
    public async Task<ActionResult<UserDto>> GetNotFound()
    {
        //-1 should never be found
        var thing = await _userBl.GetUser(-1);
        if(thing == null)
            return NotFound("Dummy user -1 not found");
        return Ok(thing);
    }

    [HttpGet("server-error")]
    public async Task<ActionResult<string>> GetServerError()
    {
        //-1 should never be found
        var thing = await _userBl.GetUser(-1);

        //should generate null reference exception
        var thingToReturn = thing.ToString(); 

        return Ok(thingToReturn);
    }

    [HttpGet("bad-request")]
    public async Task<ActionResult<string>> GetBadRequest()
    {
        //dummy call
        var thing = await _userBl.GetUser(-1);
        return BadRequest("This was not a good request!");
    }

}