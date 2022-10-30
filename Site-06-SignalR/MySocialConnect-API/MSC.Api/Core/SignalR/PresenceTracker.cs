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
    public Task UserConnected(string userName, string connectionId)
    {
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
            }
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove the user and connection on logout
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="connectionId"></param>
    /// <returns></returns>
    public Task UserDisconnected(string userName, string connectionId)
    {
        lock (_onlineUsers)
        {
            if (!_onlineUsers.ContainsKey(userName))
            {
                return Task.CompletedTask;
            }
            //remove the connection
            _onlineUsers[userName].Remove(connectionId);
            if (_onlineUsers[userName].Count == 0)
            {
                //remove the user
                _onlineUsers.Remove(userName);
            }
        }
        return Task.CompletedTask;
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
}
