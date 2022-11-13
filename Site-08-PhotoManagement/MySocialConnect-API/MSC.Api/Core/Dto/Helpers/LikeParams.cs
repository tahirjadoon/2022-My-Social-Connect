using MSC.Api.Core.Enums;

namespace MSC.Api.Core.Dto.Helpers;
public class LikeParams : PaginationParams
{
    public UserLikeType UserLikeType { get; set; }
    public int UserId { get; set; }
}
