using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;
public class MessageBusinessLogic : IMessageBusinessLogic
{
    private readonly IMessageRepository _msgRepo;
    private readonly IUsersRepository _usersRepo;
    private readonly IMapper _mapper;

    public MessageBusinessLogic(IMessageRepository msgRepo, IUsersRepository userRepo, IMapper mapper)
    {
        _msgRepo = msgRepo;
        _usersRepo = userRepo;
        _mapper = mapper;
    }

    public async Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId)
    {
        if (msg == null || msg.ReceipientUserId <= 0 || string.IsNullOrWhiteSpace(msg.Content))
            return new BusinessResponse(HttpStatusCode.BadRequest, "Message not good");

        //get source user 
        var sender = await _usersRepo.GetAppUserAsync(senderId, includePhotos: true);
        if (sender == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Logged in user not found");
        if (sender.Id == msg.ReceipientUserId)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot send message to your self");

        var receipient = await _usersRepo.GetAppUserAsync(msg.ReceipientUserId, includePhotos: true);
        if (receipient == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Receipient not found");

        var message = new Message
        {
            Sender = sender,
            Receipient = receipient,
            SenderUsername = sender.UserName,
            ReceipientUsername = receipient.UserName,
            MessageContent = msg.Content
        };

        _msgRepo.AddMessage(message);

        if (await _msgRepo.SaveAllSync())
        {
            var msgDto = _mapper.Map<MessageDto>(message);
            return new BusinessResponse(HttpStatusCode.OK, "", msgDto);
        }

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to send message");
    }

    public void DeleteMessage(Message message)
    {
        throw new System.NotImplementedException();
    }

    public Task<Message> GetMessage(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams msgParams)
    {
        var messages = await _msgRepo.GetMessagesForUser(msgParams);
        return messages;
    }


    //message between two users. Also marks receipients un read messages as read
    public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId)
    {
        var messages = await _msgRepo.GetMessageThread(currentUserId, receipientId);
        if (messages == null)
            return null;

        var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);
        return messagesDto;
    }

    public async Task<BusinessResponse> DeleteMessage(int currentUserId, int msgId)
    {
        var message = await _msgRepo.GetMessage(msgId);
        if (message.Sender.Id != currentUserId && message.Receipient.Id != currentUserId)
            return new BusinessResponse(HttpStatusCode.Unauthorized);

        //due to EF only the sender will be marked as deleted
        if (message.Sender.Id == currentUserId)
            message.SenderDeleted = true;

        //due to EF only the receipent will be marked as deleted
        if (message.Receipient.Id == currentUserId)
            message.ReceipientDeleted = true;

        //when both have deleted it then delete it altogether
        if (message.SenderDeleted && message.ReceipientDeleted)
            _msgRepo.DeleteMessage(message);

        if (await _msgRepo.SaveAllSync())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to delete message");

    }
}