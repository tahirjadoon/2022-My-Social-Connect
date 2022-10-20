using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Enums;

namespace MSC.Api.Core.Repositories;
public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        //following will not pull the receipent or sender information
        //var message = await _context.Messages.FindAsync(id);

        //we can do projectto to fill the entities or like following
        var message = await _context.Messages
                                    .Include(u => u.Receipient)
                                    .Include(u => u.Sender)
                                    .SingleOrDefaultAsync(x => x.Id == id);
        return message;
    }

    public async Task<PageList<MessageDto>> GetMessagesForUser(MessageParams msgParams)
    {
        var query = _context.Messages.OrderByDescending(m => m.DateMessageSent).AsQueryable();

        query = msgParams.MessageType switch
        {
            //receipent of the message
            MessageType.Inbox => query.Where(u => u.Receipient.Id == msgParams.UserId && !u.ReceipientDeleted),
            //receipent of the message and not read it
            MessageType.InboxUnread => query.Where(u => u.Receipient.Id == msgParams.UserId && u.DateMessageRead == null && !u.ReceipientDeleted),
            //defult sender outbox
            _ => query.Where(u => u.Sender.Id == msgParams.UserId && !u.SenderDeleted)
        };

        var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

        var pageList = await PageList<MessageDto>.CreateAsync(messages, msgParams.PageNumber, msgParams.PageSize);

        return pageList;
    }

    //message thread between two users so check for both ways. Also eagily load photos for both receipent and sender
    public async Task<IEnumerable<Message>> GetMessageThread(int currentUserId, int receipientId)
    {
        //message conversations between two users
        var messages = await _context.Messages
                                    .Include(u => u.Receipient).ThenInclude(p => p.Photos)
                                    .Include(u => u.Sender).ThenInclude(p => p.Photos)
                                    .Where(m =>
                                            (m.Receipient.Id == currentUserId && m.Sender.Id == receipientId && !m.ReceipientDeleted) ||
                                            (m.Receipient.Id == receipientId && m.Sender.Id == currentUserId && !m.SenderDeleted)
                                        )
                                    .OrderBy(m => m.DateMessageSent)
                                    .ToListAsync();
        var unreadMessages = messages.Where(m => m.DateMessageRead == null && m.Receipient.Id == currentUserId).ToList();
        if (unreadMessages != null && unreadMessages.Any())
        {
            //update the date
            unreadMessages.ForEach(x => { x.DateMessageRead = DateTime.Now; });
            await _context.SaveChangesAsync();
        }

        return messages;
    }

    public async Task<bool> SaveAllSync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}