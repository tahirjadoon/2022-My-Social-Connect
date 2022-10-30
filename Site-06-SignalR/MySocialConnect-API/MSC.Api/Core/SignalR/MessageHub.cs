using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Extensions;

//no package to install for SignalR
namespace MSC.Api.Core.SignalR;

[Authorize]
/// <summary>
/// MessageHub, it derives from Hub and then override the virtual methods 
/// </summary>
public class MessageHub : Hub
{
    private readonly IMessageBusinessLogic _msgBl;
    public MessageHub(IMessageBusinessLogic msgBl)
    {
        _msgBl = msgBl;
    }

    /// <summary>
    /// Implement OnConnectedAsync 
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        //create a group of two users. will be passing in the other users name 
        var httpContext = Context.GetHttpContext();
        var otherUserName = httpContext.Request.Query["otherUserName"].ToString();
        var otherUserId = int.Parse(httpContext.Request.Query["otherUserId"].ToString());
        //build the group name
        var groupName = GetGroupName(Context.User.GetUserName(), otherUserName);
        //add to group
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //when the user joins the group get the messages 
        var messages = await _msgBl.GetMessageThread(Context.User.GetUserId(), otherUserId);
        //send the message
        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

    }

    /// <summary>
    /// Implmenent OnDisconnectedAsync 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        //users will be automatically removed from the group 
        await base.OnDisconnectedAsync(exception);
    }

    //copied the code from the MessageController CreateMessage action
    public async Task SendMessage(MessageCreateDto msg)
    {
        //get the claims
        var userClaims = Context.User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            throw new HubException("User issue");

        var result = await _msgBl.AddMessage(msg, userClaims.UserId);
        if (result == null)
            throw new HubException("Unable to send message");

        if (result.HttpStatusCode != HttpStatusCode.OK)
            throw new HubException(result.Message ?? "Unable to send message");

        var message = result.ConvertDataToType<MessageDto>();
        var groupName = GetGroupName(message.SenderUsername, message.ReceipientUsername);
        await Clients.Group(groupName).SendAsync("NewMessage", message);

    }

    private string GetGroupName(string caller, string other)
    {
        //Less than zero –strA is less than strB.
        //Zero –strA and strB are equal.
        //Greater than zero –strA is greater than strB
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}