using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Controllers;

[Authorize]
public class LikesController : BaseApiController
{
    private readonly IUsersBusinessLogic _userBl;
    private readonly ILikesBusinessLogic _likesBl;


    public LikesController(IUsersBusinessLogic userBl, ILikesBusinessLogic likesBl)
    {
        _userBl = userBl;
        _likesBl = likesBl;
    }

    [HttpPost("{likeId:int}/like/{name}")]
    public async Task<ActionResult> AddLike(int likeId, string name)
    {
        if (likeId <= 0)
        {
            return BadRequest("The user to like is required");
        }

        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        var result = await _likesBl.AddLike(likeId, userClaims);

        ActionResult actionResult = BadRequest("Unable to add like");
        if (result != null)
        {
            switch (result.HttpStatusCode)
            {
                case HttpStatusCode.OK:
                    actionResult = Ok();
                    break;
                case HttpStatusCode.BadRequest:
                    actionResult = BadRequest(result.Message);
                    break;
                case HttpStatusCode.NotFound:
                    actionResult = NotFound(result.Message);
                    break;
                default:
                    actionResult = BadRequest("Unable to add like");
                    break;
            }
        }

        return actionResult;
    }

    [HttpGet("user/likes")]
    public async Task<ActionResult<IEnumerable<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
        {
            return BadRequest("User issue");
        }

        likeParams.UserId = userClaims.UserId;

        var users = await _likesBl.GetUserLikes(likeParams);
        if (users == null) //dont check !likes.Any(), good with Ok result with empty array
            return NotFound();

        //users has the pagination information so will need to write the pagination header using the extension we created
        Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

        return Ok(users);
    }

}
