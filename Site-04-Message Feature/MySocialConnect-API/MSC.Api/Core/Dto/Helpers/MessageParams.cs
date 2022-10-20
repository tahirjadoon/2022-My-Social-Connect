using MSC.Api.Core.Enums;

namespace MSC.Api.Core.Dto.Helpers;
public class MessageParams : PaginationParams
{
    public int UserId { get; set; }
    public MessageType MessageType { get; set; } = MessageType.InboxUnread;
}