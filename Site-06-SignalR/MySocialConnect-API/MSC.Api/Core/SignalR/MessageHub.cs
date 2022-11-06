using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSC.Api.Core.BusinessLogic;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;
using MSC.Api.Core.Repositories;

//no package to install for SignalR
namespace MSC.Api.Core.SignalR;

[Authorize]
/// <summary>
/// MessageHub, it derives from Hub and then override the virtual methods 
/// </summary>
public class MessageHub : Hub
{
    private readonly IMessageBusinessLogic _msgBl;
    private readonly ISignalRBusinessLogic _signalrBl;
    private readonly IUsersRepository _usersRepo;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _presenceTracker;
    private readonly IMapper _mapper;

    public MessageHub(IUsersRepository userRepo,
        IMessageBusinessLogic msgBl,
        ISignalRBusinessLogic signalrBl,
        IHubContext<PresenceHub> presenceHub,
        PresenceTracker presenceTracker,
        IMapper mapper)
    {
        _usersRepo = userRepo;
        _msgBl = msgBl;
        _signalrBl = signalrBl;
        _presenceHub = presenceHub;
        _presenceTracker = presenceTracker;
        _mapper = mapper;
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
        //persist in DB
        SignalRGroup group = await AddToGroup(groupName);
        //pass the group back 
        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        //when the user joins the group get the messages 
        var messages = await _msgBl.GetMessageThread(Context.User.GetUserId(), otherUserId);
        //send the message to the caller
        //await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

    }

    /// <summary>
    /// Implmenent OnDisconnectedAsync 
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        //remove from the DB
        SignalRGroup group = await RemoveFromMessageGroup();
        //pass the group back 
        await Clients.Group(group.GroupName).SendAsync("UpdatedGroup", group);
        //users will be automatically removed from the group 
        await base.OnDisconnectedAsync(exception);
    }

    //copied the code from the MessageController CreateMessage action and then updated it
    public async Task SendMessage(MessageCreateDto msg)
    {
        //get the claims
        var userClaims = Context.User.GetUserClaims();
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            throw new HubException("User issue");

        //build message 
        Message message = await BuildMessage(msg, Context, userClaims);
        if (message == null)
            throw new HubException("Message not built");

        //group info and group
        var groupName = GetGroupName(message.SenderUsername, message.ReceipientUsername);
        SignalRGroup group = await _signalrBl.GetMessageGroup(groupName);

        //mark the message as read 
        if (group.Connections.Any(x => x.UserName == message.ReceipientUsername))
        {
            message.DateMessageRead = DateTime.UtcNow;
        }
        else
        {
            //target receipient of the message is online but not on the messages page. User is on any other page so show notification
            var connections = await _presenceTracker.GetConnectionsForUser(message.ReceipientUsername);
            if (connections != null)
            {
                //send notification
                var user = _mapper.Map<UserDto>(message.Sender);
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", user);
            }
        }

        //add message to db
        var result = await _msgBl.AddMessage(message);
        if (result == null)
            throw new HubException("Unable to send message");
        if (result.HttpStatusCode != HttpStatusCode.OK)
            throw new HubException(result.Message ?? "Unable to send message");

        //we have the message that got added to the database in result so pick it
        var messageAdded = result.ConvertDataToType<MessageDto>();

        //return the added message 
        await Clients.Group(groupName).SendAsync("NewMessage", messageAdded);
    }

    private async Task<Message> BuildMessage(MessageCreateDto msg, HubCallerContext context, UserClaimGetDto userClaims)
    {
        if (userClaims == null || (!userClaims.HasGuid || !userClaims.HasUserName))
            throw new HubException("User issue");

        if (msg == null || msg.ReceipientUserId <= 0 || string.IsNullOrWhiteSpace(msg.Content))
            throw new HubException("Message info invalid");

        //sender user 
        var sender = await _usersRepo.GetAppUserAsync(userClaims.UserId, includePhotos: true);
        if (sender == null)
            throw new HubException("Logged in user not found");

        //receipient
        var receipient = await _usersRepo.GetAppUserAsync(msg.ReceipientUserId, includePhotos: true);
        if (receipient == null)
            throw new HubException("Receipient not found");

        var message = new Message
        {
            Sender = sender,
            Receipient = receipient,
            SenderUsername = sender.UserName,
            ReceipientUsername = receipient.UserName,
            MessageContent = msg.Content
        };

        return message;
    }

    private string GetGroupName(string caller, string other)
    {
        //Less than zero –strA is less than strB.
        //Zero –strA and strB are equal.
        //Greater than zero –strA is greater than strB
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    }

    /// <summary>
    /// Add to group
    /// </summary>
    /// <param name="groupName"></param>
    /// <returns></returns>
    private async Task<SignalRGroup> AddToGroup(string groupName)
    {
        //logged in user name and id
        var userName = Context.User.GetUserName();
        var userId = Context.User.GetUserId();

        //get the group from the db
        SignalRGroup group = await _signalrBl.GetMessageGroup(groupName);

        //create a connection
        SignalRConnection connection = new SignalRConnection(Context.ConnectionId, userName, userId);

        //logic
        if (group == null)
        {
            group = new SignalRGroup(groupName);
            _signalrBl.AddGroup(group);
        }
        group.Connections.Add(connection);

        //save
        if (await _signalrBl.SaveAllAsync())
            return group;

        throw new HubException("Failed to join group");
    }

    /// <summary>
    /// Remove connection from Message group
    /// </summary>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    private async Task<SignalRGroup> RemoveFromMessageGroup()
    {
        SignalRGroup group = await _signalrBl.GetGroupForConnection(Context.ConnectionId);
        if (group == null)
            throw new HubException("Failed to get group for connection");
        SignalRConnection connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        if (connection == null)
            throw new HubException("Failed to get connection");
        _signalrBl.RemoveConnection(connection);
        if (await _signalrBl.SaveAllAsync())
            return group;
        throw new HubException("Failed to remove from group");
    }
}