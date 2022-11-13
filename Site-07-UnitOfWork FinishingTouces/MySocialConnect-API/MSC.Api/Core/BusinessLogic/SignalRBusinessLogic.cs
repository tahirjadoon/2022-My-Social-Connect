using System.Threading.Tasks;
using MSC.Api.Core.DB.UnitOfWork;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;
public class SignalRBusinessLogic : ISignalRBusinessLogic
{
    private readonly IUnitOfWork _uow;

    public SignalRBusinessLogic(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public void AddGroup(SignalRGroup group)
    {
        _uow.SignalRRepo.AddGroup(group);
    }

    public async Task<SignalRConnection> GetConnection(string connectionId)
    {
        return await _uow.SignalRRepo.GetConnection(connectionId);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
        return await _uow.SignalRRepo.GetMessageGroup(groupName);
    }

    public async Task<SignalRGroup> GetGroupForConnection(string connectionId)
    {
        return await _uow.SignalRRepo.GetGroupForConnection(connectionId);
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _uow.SignalRRepo.RemoveConnection(connection);

    }

    public async Task<bool> SaveAllAsync()
    {
        return await _uow.Complete();
    }
}
