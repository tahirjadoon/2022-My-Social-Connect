using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Controllers;
public class AdminController : BaseApiController
{
    private readonly IUsersBusinessLogic _userBl;

    public AdminController(IUsersBusinessLogic userBl)
    {
        _userBl = userBl;
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_RequireAdminRole)]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<IEnumerable<object>>> GetUsersWithRoles()
    {
        var users = await _userBl.GetUSersWithRoles();
        return Ok(users);
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_ModeratePhotoRole)]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosForModeration()
    {
        return Ok("Admins or moderators can see this");
    }

    [Authorize(Policy = SiteIdentityConstants.AuthPolicy_RequireAdminRole)]
    [HttpPost("edit-roles/{guid:Guid}")]
    public async Task<ActionResult<IEnumerable<string>>> EditRoles([FromRoute] Guid guid, [FromQuery] string roles)
    {
        if (string.IsNullOrWhiteSpace(roles))
            return BadRequest("No roles provided to update");

        var rolesList = roles.StringSplitToType<string>();
        if (rolesList == null || !rolesList.Any())
            return BadRequest("Unable to parse the roles provided");

        //edit the roles 
        var result = await _userBl.EditRolesForUser(User.GetUserId(), guid, rolesList);

        ActionResult actionResult = BadRequest("Unable to edit roles");
        if (result != null)
        {
            switch (result.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    actionResult = Ok(result.ConvertDataToType<IEnumerable<string>>());
                    break;
                case HttpStatusCode.BadRequest:
                    actionResult = BadRequest(result.Message);
                    break;
                case HttpStatusCode.NotFound:
                    actionResult = NotFound(result.Message);
                    break;
                default:
                    actionResult = BadRequest("Unable to edit roles");
                    break;
            }
        }

        return actionResult;
    }
}