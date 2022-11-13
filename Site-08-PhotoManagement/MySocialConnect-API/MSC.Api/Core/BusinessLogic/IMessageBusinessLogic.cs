using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;
public interface IMessageBusinessLogic
{
    Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId);
    Task<BusinessResponse> AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PageList<MessageDto>> GetMessagesForUser(MessageParams msgParams);
    Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId);
    Task<BusinessResponse> DeleteMessage(int currentUserId, int msgId);
}
