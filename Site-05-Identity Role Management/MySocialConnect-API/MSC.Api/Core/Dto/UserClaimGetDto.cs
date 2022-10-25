using System;
using System.Collections.Generic;

namespace MSC.Api.Core.Dto;
public class UserClaimGetDto
{
    public string UserName { get; set; }
    public int UserId { get; set; }
    public Guid Guid { get; set; }
    public string DisplayName { get; set; }
    public List<string> Roles { get; set; }

    public bool HasUserName => !string.IsNullOrWhiteSpace(UserName);
    public bool HasGuid => Guid != Guid.Empty;
    public bool HasId => UserId > 0;

    public bool HasPrimaryInfo => HasUserName && HasGuid && HasId;
}