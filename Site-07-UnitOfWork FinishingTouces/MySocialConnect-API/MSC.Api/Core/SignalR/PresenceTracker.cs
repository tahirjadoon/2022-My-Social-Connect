//1. starting with local presence tracker. The elaborate one would be with redis or in database

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSC.Api.Core.SignalR;

/// <summary>
/// Add the service as singleton in 
/// </summary>
public class PresenceTracker
{
    //dictionary to hold the userKey and the list of connections
    private static readonly Dictionary<string, List<string>> _onlineUsers = new Dictionary<string, List<string>>();

    /// <summary>
    /// Add to the user and connectionId on login
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public Task<bool> UserConnected(string userName, string connectionId)
    {
        var isOnline = false;
        lock (_onlineUsers)
        {
            if (_onlineUsers.ContainsKey(userName))
            {
                //add the connectionId to the second part of the dictionary
                _onlineUsers[userName].Add(connectionId);
            }
            else
            {
                //add the the user name with the connection id
                _onlineUsers.Add(userName, new List<string> { connectionId });
                //user has come online so update the flag
                isOnline = true;
            }
        }
        //return Task.CompletedTask;
        return Task.FromResult(isOnline);
    }

    /// <summary>
    /// Remove the user and connection on logout
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public Task<bool> UserDisconnected(string userName, string connectionId)
    {
        var isOffline = false;
        lock (_onlineUsers)
        {
            if (!_onlineUsers.ContainsKey(userName))
            {
                //return Task.CompletedTask;
                return Task.FromResult(isOffline);
            }
            //remove the connection
            _onlineUsers[userName].Remove(connectionId);
            if (_onlineUsers[userName].Count == 0)
            {
                //remove the user
                _onlineUsers.Remove(userName);
                //when the user is completely removed then make isoffline true
                isOffline = true;
            }
        }
        //return Task.CompletedTask;
        return Task.FromResult(isOffline);
    }

    /// <summary>
    /// Get the list of online users
    /// </summary>
    /// <returns></returns>
    public Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsers;
        lock (_onlineUsers)
        {
            onlineUsers = _onlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
        }
        return Task.FromResult(onlineUsers);
    }

    /// <summary>
    /// Get all connections for the user user in MessageHub
    /// </summary>
    /// <returns></returns>
    public Task<List<string>> GetConnectionsForUser(string userName)
    {
        List<string> connectionIds;
        lock (_onlineUsers)
        {
            connectionIds = _onlineUsers.GetValueOrDefault(userName);
        }
        return Task.FromResult(connectionIds);
    }
}
