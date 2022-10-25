using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;
public interface ILikesBusinessLogic
{
    Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);

    Task<AppUser> GetUserWithLikes(int userId);

    Task<PageList<LikeDto>> GetUserLikes(LikeParams likeParams);

    Task<BusinessResponse> AddLike(int likeId, UserClaimGetDto claims);
}
