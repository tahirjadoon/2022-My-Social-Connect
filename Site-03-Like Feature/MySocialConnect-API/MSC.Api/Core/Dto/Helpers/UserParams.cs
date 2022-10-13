using System;
using MSC.Api.Core.Constants;

namespace MSC.Api.Core.Dto.Helpers;
public class UserParams : PaginationParams
{
    //user filtering parameters
    public Guid? CurrentUserGuid { get; set; }
    public string Gender { get; set; }
    public int MinAge { get; set; } = DataConstants.MinAge;
    public int MaxAge { get; set; } = DataConstants.MaxAge;
    public string OrderBy { get; set; } = DataConstants.LastActive;
}
