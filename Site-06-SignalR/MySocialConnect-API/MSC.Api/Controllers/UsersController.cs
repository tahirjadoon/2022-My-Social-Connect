using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Extensions;

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
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers([FromQuery] UserParams userParams)
    {
        //get the current user
        var user = await _usersBl.GetUserAsync(User.GetUserName());
        if (user == null)
        {
            return BadRequest("User issue");
        }
        //for removing the curren tuser from the list
        userParams.CurrentUserGuid = user.GuId;
        //by default picking the opposite gender when not supplied
        if (string.IsNullOrWhiteSpace(userParams.Gender))
            userParams.Gender = user.Gender.ToUpperInvariant() == "MALE" ? "female" : "male";

        var users = await _usersBl.GetUsersAsync(userParams);
        if (users == null || !users.Any())
        {
            return NotFound("No users found!");
        }

        //users has the pagination information so will need to write the pagination header using the extension we created
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        return Ok(users);
    }

    [HttpGet("{guid}/guid", Name = "GetUserByGuid")]
    public async Task<ActionResult<UserDto>> GetUser(Guid guid)
    {
        var user = await _usersBl.GetUserByGuidAsync(guid);
        if (user == null)
        {
            return NotFound($"No user found by guid {guid}");
        }

        return Ok(user);
    }

    [HttpGet("{id}/id", Name = "GetUserById")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var user = await _usersBl.GetUserAsync(id);
        if (user == null)
        {
            return NotFound($"No user found by id {id}");
        }

        return Ok(user);
    }

    [HttpGet("{name}/name", Name = "GetUserByName")]
    public async Task<ActionResult<UserDto>> GetUser(string name)
    {
        var user = await _usersBl.GetUserAsync(name);
        if (user == null)
        {
            return NotFound($"No user found by name {name}");
        }

        return Ok(user);
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser([FromBody] UserUpdateDto userUpdateDto)
    {
        //get the claims
        //var userClaims = base.GetLoggedInCalims();
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        var isUpdate = await _usersBl.UpdateUserAsync(userUpdateDto, userClaims);
        if (!isUpdate)
        {
            return BadRequest("User not updated");
        }

        return NoContent();
    }

    [HttpPost("add/photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        //get claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        var photoDto = await _usersBl.AddPhoto(file, userClaims);
        if (photoDto == null)
            return BadRequest("Problem adding photo");

        //this is to tell from where to pick the info. Point to GetUserByGuid and pass the guid to it
        //this will return 401
        //lok at the location headers for the for url to the action that gets the user by guid
        return CreatedAtRoute("GetUserByGuid", new { guid = userClaims.Guid.ToString() }, photoDto);
    }

    [HttpPut("set/photo/{photoId}/main")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        //get claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        var result = await _usersBl.SetPhotoMain(photoId, userClaims);

        if (result)
            return NoContent();

        return BadRequest("Unable to set photo to main");
    }

    [HttpDelete("delete/{photoId}/photo")]
    public async Task<ActionResult> DeletePhoto(int photoId)
    {
        //get claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        var result = await _usersBl.DeletePhoto(photoId, userClaims);

        ActionResult actionResult = BadRequest("Unable to delete photo");
        if (result != null)
        {
            switch (result.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    actionResult = Ok();
                    break;
                case HttpStatusCode.BadRequest:
                    actionResult = BadRequest(result.Message ?? "Unable to delete photo");
                    break;
                case HttpStatusCode.NotFound:
                    actionResult = NotFound(result.Message ?? "Photo not found");
                    break;
                default:
                    actionResult = BadRequest("Unable to delete photo");
                    break;
            }
        }

        return actionResult;
    }
}