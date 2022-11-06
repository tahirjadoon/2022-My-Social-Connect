using System.Threading.Tasks;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;
public class SignalRBusinessLogic : ISignalRBusinessLogic
{
    private readonly ISignalRRepository _srRepo;

    public SignalRBusinessLogic(ISignalRRepository srRepo)
    {
        _srRepo = srRepo;
    }

    public void AddGroup(SignalRGroup group)
    {
        _srRepo.AddGroup(group);
    }

    public async Task<SignalRConnection> GetConnection(string connectionId)
    {
        return await _srRepo.GetConnection(connectionId);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
        return await _srRepo.GetMessageGroup(groupName);
    }

    public async Task<SignalRGroup> GetGroupForConnection(string connectionId)
    {
        return await _srRepo.GetGroupForConnection(connectionId);
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _srRepo.RemoveConnection(connection);

    }

    public async Task<bool> SaveAllAsync()
    {
        return await _srRepo.SaveAllAsync();
    }
}
