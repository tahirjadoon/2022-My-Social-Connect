namespace MSC.Api.Core.Entities;
public class UserLike
{
    //fully defining the relationship between AppUser and UserLike. CheckDB Context for relationships
    public AppUser SourceUser { get; set; }
    public int SourceUserId { get; set; }

    //fully defining the relationship between AppUser and UserLike. CheckDB Context for relationships
    public AppUser LikedUser { get; set; }
    public int LikedUserId { get; set; }
}
