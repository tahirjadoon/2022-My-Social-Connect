using System;

namespace MSC.Api.Core.Entities;
public class Message
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public AppUser Sender { get; set; }
    public bool SenderDeleted { get; set; }
    public int ReceipientId { get; set; }
    public string ReceipientUsername { get; set; }
    public AppUser Receipient { get; set; }
    public bool ReceipientDeleted { get; set; }
    public string MessageContent { get; set; }
    public DateTime? DateMessageRead { get; set; }
    public DateTime DateMessageSent { get; set; } = DateTime.UtcNow;
}
