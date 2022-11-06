using System.ComponentModel.DataAnnotations;

namespace MSC.Api.Core.Entities;
public class SignalRConnection
{
    /// <summary>
    /// Empty constructory is needed for the EF
    /// </summary>
    public SignalRConnection()
    {
    }

    public SignalRConnection(string connectionId, string userName, int userId)
    {
        ConnectionId = connectionId;
        UserName = userName;
        UserId = userId;
    }

    [Key]
    public string ConnectionId { get; set; }
    public string UserName { get; set; }
    public int UserId { get; set; }
}
