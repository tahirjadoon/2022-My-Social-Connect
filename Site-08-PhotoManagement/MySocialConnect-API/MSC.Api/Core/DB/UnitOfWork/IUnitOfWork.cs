using System.Threading.Tasks;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.DB.UnitOfWork;
public interface IUnitOfWork
{
    IUsersRepository UsersRepo { get; }
    ILikesRepository LikesRepo { get; }
    IMessageRepository MessageRepo { get; }
    ISignalRRepository SignalRRepo { get; }
    IPhotoRepository PhotoRepo { get; }

    Task<bool> Complete();
    bool HasChanges();
}
