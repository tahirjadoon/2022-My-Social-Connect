using System.Threading.Tasks;
using AutoMapper;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.DB.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UnitOfWork(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public IUsersRepository UsersRepo => new UsersRepository(_context, _mapper);

    public ILikesRepository LikesRepo => new LikesRepository(_context);

    public IMessageRepository MessageRepo => new MessageRepository(_context, _mapper);

    public ISignalRRepository SignalRRepo => new SignalRRepository(_context, _mapper);

    public async Task<bool> Complete()
    {
        return await _context.SaveChangesAsync() > 0;
    }

    public bool HasChanges()
    {
        return _context.ChangeTracker.HasChanges();
    }
}