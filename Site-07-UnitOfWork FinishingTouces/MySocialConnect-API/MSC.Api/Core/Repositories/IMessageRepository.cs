using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;
public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PageList<MessageDto>> GetMessagesForUser(MessageParams msgParams);
    Task<IEnumerable<Message>> GetMessageThread(int currentUserId, int receipientId);
    Task<bool> SaveAllSync();
}