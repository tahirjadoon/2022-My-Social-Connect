using System;

namespace MSC.Api.Core.Dto;

public class UserTokenDto
{
    public string UserName { get; set; }
    public Guid GuId { get; set; }
    public string Token { get; set; }
}