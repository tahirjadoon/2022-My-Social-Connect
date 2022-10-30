using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using MSC.Api.Core.Extensions;

//no package to install for SignalR
namespace MSC.Api.Core.SignalR;

[Authorize]
/// <summary>
/// PresenceHub, it derives from Hub and then override the virtual methods 
/// </summary>
public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;
    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
    }

    /// <summary>
    /// Implement OnConnectedAsync to tell other users when the current user goes online
    /// </summary>
    /// <returns></returns>
    public override async Task OnConnectedAsync()
    {
        //pick the current user from Token ==> claims
        //add the user to presenceTracker
        //other than the current user tell all others that the user is online
        var userName = Context.User.GetUserName();
        var connectionId = Context.ConnectionId;
        await _tracker.UserConnected(userName, connectionId);
        await Clients.Others.SendAsync("UserIsOnline", userName);

        //get the users online and send to every one who is connected
        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);
    }

    /// <summary>
    /// Implmenent OnDisconnectedAsync to tell other users when the current user goes offline
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        //pick the current user from Token ==> claims
        //remove the user from presenceTracker
        //tell all other users when the use goes offline
        var userName = Context.User.GetUserName();
        var connectionId = Context.ConnectionId;
        await _tracker.UserDisconnected(userName, connectionId);
        await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserName());

        //get the users online and send to every one who is connected
        var currentUsers = await _tracker.GetOnlineUsers();
        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnDisconnectedAsync(exception);
    }
}
