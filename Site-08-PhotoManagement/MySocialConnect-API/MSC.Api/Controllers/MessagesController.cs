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
public class MessagesController : BaseApiController
{
    private readonly IMessageBusinessLogic _msgBl;

    public MessagesController(IMessageBusinessLogic msgBl)
    {
        _msgBl = msgBl;
    }

    [HttpPost("send/message")]
    public async Task<ActionResult<MessageDto>> CreateMessage([FromBody] MessageCreateDto msg)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User issue");

        var result = await _msgBl.AddMessage(msg, userClaims.UserId);
        if (result == null)
            return BadRequest("Unable to send message");

        ActionResult actionResult = BadRequest("Unable to send message");
        switch (result.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                actionResult = Ok(result.ConvertDataToType<MessageDto>());
                break;
            case HttpStatusCode.BadRequest:
                actionResult = BadRequest(result.Message);
                break;
            case HttpStatusCode.NotFound:
                actionResult = NotFound(result.Message);
                break;
            default:
                actionResult = BadRequest("Unable to send message");
                break;
        }

        return actionResult;
    }

    [HttpGet("user/get/messages")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessagesForUser([FromQuery] MessageParams msgParams)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User issue");
        msgParams.UserId = userClaims.UserId;
        var messages = await _msgBl.GetMessagesForUser(msgParams);
        if (messages == null)
            return NotFound("No messages found");

        Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

        return Ok(messages);
    }

    [HttpGet("message/thread/{otherUserId:int}")]
    public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread([FromRoute] int otherUserId)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User issue");

        var messages = await _msgBl.GetMessageThread(userClaims.UserId, otherUserId);
        if (messages == null)
            return NotFound("No messages found");

        return Ok(messages);
    }

    [HttpDelete("delete/message/{id:int}")]
    public async Task<ActionResult> DeleteMessage([FromRoute] int id)
    {
        //get the claims
        var userClaims = User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            return BadRequest("User issue");

        var result = await _msgBl.DeleteMessage(userClaims.UserId, id);
        if (result == null)
            return BadRequest("Unable to delete message");

        ActionResult actionResult = BadRequest("Unable to delete message");
        switch (result.HttpStatusCode)
        {
            case HttpStatusCode.OK:
                actionResult = Ok(result.ConvertDataToType<MessageDto>());
                break;
            case HttpStatusCode.BadRequest:
                actionResult = BadRequest(result.Message);
                break;
            case HttpStatusCode.NotFound:
                actionResult = NotFound(result.Message);
                break;
            default:
                actionResult = BadRequest("Unable to delete message");
                break;
        }

        return actionResult;
    }
}