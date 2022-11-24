using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;
public class SignalRRepository : ISignalRRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public SignalRRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddGroup(SignalRGroup group)
    {
        _context.SignalRGroups.Add(group);
    }

    public async Task<SignalRConnection> GetConnection(string connectionId)
    {
        var connection = await _context.SignalRConnections.FindAsync(connectionId);
        return connection;
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
        //also fill in the connections for the group
        var group = await _context.SignalRGroups
                                    .Include(x => x.Connections)
                                    .FirstOrDefaultAsync(x => x.GroupName == groupName);
        return group;
    }

    public async Task<SignalRGroup> GetGroupForConnection(string connectionId)
    {
        return await _context.SignalRGroups
                            .Include(x => x.Connections)
                            .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
                            .FirstOrDefaultAsync();
    }

    public void RemoveConnection(SignalRConnection connection)
    {
        _context.SignalRConnections.Remove(connection);
    }

    /*
    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    */
}