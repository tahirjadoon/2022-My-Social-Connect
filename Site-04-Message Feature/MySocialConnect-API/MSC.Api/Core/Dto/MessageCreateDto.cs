namespace MSC.Api.Core.Dto;
public class MessageCreateDto
{
    public int ReceipientUserId { get; set; }
    public string Content { get; set; }
}