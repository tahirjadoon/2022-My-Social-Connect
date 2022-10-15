using System;

namespace MSC.Api.Core.Dto;

public class UserTokenDto
{
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public Guid GuId { get; set; }
    public string Token { get; set; }
    public string MainPhotoUrl { get; set; }
    public string Gender { get; set; }
}