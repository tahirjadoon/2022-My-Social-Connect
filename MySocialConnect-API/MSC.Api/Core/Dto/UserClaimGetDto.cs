using System;

namespace MSC.Api.Core.Dto;
public class UserClaimGetDto
{
    public string UserName { get; set; }
    public Guid Guid { get; set; }
    public string DisplayName { get; set; }

    public bool HasUserName => !string.IsNullOrWhiteSpace(UserName);
    public bool HasGuid => Guid != Guid.Empty;
}