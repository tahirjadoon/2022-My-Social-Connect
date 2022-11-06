using System.Threading.Tasks;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;
public interface ISignalRBusinessLogic
{
    void AddGroup(SignalRGroup group);
    void RemoveConnection(SignalRConnection connection);
    Task<SignalRConnection> GetConnection(string connectionId);
    Task<SignalRGroup> GetMessageGroup(string groupName);
    Task<SignalRGroup> GetGroupForConnection(string connectionId);
    Task<bool> SaveAllAsync();
}
