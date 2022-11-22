using System;

namespace MSC.Api.Core.Dto;

public class PhotoForApprovalDto
{
    public int Id { get; set; }
    public string Url { get; set; }
    public string Username { get; set; }
    public int UserId { get; set; }
    public Guid UserGuid { get; set; }
    public bool IsApproved { get; set; }
}
