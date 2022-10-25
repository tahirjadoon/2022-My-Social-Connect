using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Enums;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Core.Repositories;
public class LikesRepository : ILikesRepository
{
    public readonly DataContext _context;

    public LikesRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
    {
        var like = await _context.Likes.FindAsync(sourceUserId, likedUserId);
        return like;
    }

    public async Task<AppUser> GetUserWithLikes(int userId)
    {
        var user = await _context.Users
                                    .Include(x => x.UsersILiked)
                                    .FirstOrDefaultAsync(x => x.Id == userId);
        return user;
    }

    public async Task<PageList<LikeDto>> GetUserLikes(LikeParams likeParams)
    {
        var usersQuery = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var likesQuery = _context.Likes.AsQueryable();

        switch (likeParams.UserLikeType)
        {
            case UserLikeType.Liked:
                //users liked by the logged in user
                likesQuery = likesQuery.Where(l => l.SourceUserId == likeParams.UserId);
                usersQuery = likesQuery.Select(l => l.LikedUser);
                break;
            case UserLikeType.LikedBy:
                //others users have liked the logged in user 
                likesQuery = likesQuery.Where(l => l.LikedUserId == likeParams.UserId);
                usersQuery = likesQuery.Select(l => l.SourceUser);
                break;
            default:
                throw new ValidationException($"Unable to GetUserLikes as UserLikeType '{likeParams.UserLikeType.ToString()}' is not known");
                //break;
        }

        //project into likeDto
        var likes = usersQuery.Select(user => new LikeDto
        {
            UserName = user.UserName,
            DisplayName = user.DisplayName,
            Age = user.DateOfBirth.CalculateAge(),
            PhotoUrl = user.Photos.FirstOrDefault(u => u.IsMain).Url,
            GuId = user.GuId,
            City = user.City,
            Id = user.Id
        });

        var users = await PageList<LikeDto>.CreateAsync(likes, likeParams.PageNumber, likeParams.PageSize);

        return users;
    }
}
