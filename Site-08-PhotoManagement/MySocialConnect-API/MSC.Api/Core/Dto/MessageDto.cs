using System;
using System.Text.Json.Serialization;

namespace MSC.Api.Core.Dto;
public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public Guid SenderGuid { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; }
    public int ReceipientId { get; set; }
    public Guid ReceipientGuid { get; set; }
    public string ReceipientUsername { get; set; }
    public string ReceipientPhotoUrl { get; set; }
    public string MessageContent { get; set; }
    public DateTime? DateMessageRead { get; set; }
    public DateTime DateMessageSent { get; set; }

    [JsonIgnore]
    public bool ReceipientDeleted { get; set; }

    [JsonIgnore]
    public bool SenderDeleted { get; set; }
}