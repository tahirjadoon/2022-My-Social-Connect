using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MSC.Api.Core.DB.UnitOfWork;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Repositories;

namespace MSC.Api.Core.BusinessLogic;
public class LikesBusinessLogic : ILikesBusinessLogic
{
    private readonly IUnitOfWork _uow;

    public LikesBusinessLogic(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
    {
        var like = await _uow.LikesRepo.GetUserLike(sourceUserId, likedUserId);
        return like;
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        var user = await _uow.LikesRepo.GetUserWithLikes(userId);
        return user;
    }

    public async Task<PageList<LikeDto>> GetUserLikes(LikeParams likeParams)
    {
        var users = await _uow.LikesRepo.GetUserLikes(likeParams);
        return users;
    }

    public async Task<BusinessResponse> AddLike(int likeId, UserClaimGetDto claims)
    {
        //get source user 
        var sourceUser = await _uow.UsersRepo.GetAppUserAsync(claims.UserId, includePhotos: false);
        if (sourceUser == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Logged in user not found");


        //get liked user 
        var likedUser = await _uow.UsersRepo.GetAppUserAsync(likeId, includePhotos: false);
        if (likedUser == null)
            return new BusinessResponse(HttpStatusCode.NotFound, "Liked user not found");

        if (likedUser.UserName == sourceUser.UserName)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You cannot like yourself");

        var userLike = await _uow.LikesRepo.GetUserLike(sourceUser.Id, likedUser.Id);
        if (userLike != null)
            return new BusinessResponse(HttpStatusCode.BadRequest, "You already liked this user");

        //save - add to the source user
        userLike = new UserLike { SourceUserId = sourceUser.Id, LikedUserId = likedUser.Id };
        if (sourceUser.UsersILiked == null) sourceUser.UsersILiked = new List<UserLike>();
        sourceUser.UsersILiked.Add(userLike);
        if (await _uow.Complete())
            return new BusinessResponse(HttpStatusCode.OK);

        return new BusinessResponse(HttpStatusCode.BadRequest, "Unable to add like");
    }
}
