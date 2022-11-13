using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MSC.Api.Core.DB.UnitOfWork;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;
public class MessageBusinessLogic : IMessageBusinessLogic
{
    private readonly IMapper _mapper;

    private readonly IUnitOfWork _uow;

    public MessageBusinessLogic(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<BusinessResponse> AddMessage(MessageCreateDto msg, int senderId)
    {
        if (msg == null || msg.ReceipientUserId <= 0 || string.IsNullOrWhiteSpace(msg.Content))
            return new BusinessResponse(HttpStatusCode.BadRequest, "Message not good");

        //get source user 
        var sender = await _uow.UsersRepo.GetAppUserAsync(senderId, includePhotos: true);
        if (sender == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Logged in user not found");
        if (sender.Id == msg.ReceipientUserId)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot send message to your self");

        var receipient = await _uow.UsersRepo.GetAppUserAsync(msg.ReceipientUserId, includePhotos: true);
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

        var result = await AddMessage(message);
        return result;
    }

    public async Task<BusinessResponse> AddMessage(Message message)
    {
        _uow.MessageRepo.AddMessage(message);

        if (await _uow.Complete())
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
        var messages = await _uow.MessageRepo.GetMessagesForUser(msgParams);
        return messages;
    }


    //message between two users. Also marks receipients un read messages as read
    public async Task<IEnumerable<MessageDto>> GetMessageThread(int currentUserId, int receipientId)
    {
        var messages = await _uow.MessageRepo.GetMessageThread(currentUserId, receipientId);
        if (messages == null)
            return null;

        if (_uow.HasChanges())
            await _uow.Complete();

        //var messagesDto = _mapper.Map<IEnumerable<MessageDto>>(messages);
        //return messagesDto;
        return messages;
    }

    public async Task<BusinessResponse> DeleteMessage(int currentUserId, int msgId)
    {
        var message = await _uow.MessageRepo.GetMessage(msgId);
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
            _uow.MessageRepo.DeleteMessage(message);

        if (await _uow.Complete())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to delete message");

    }
}