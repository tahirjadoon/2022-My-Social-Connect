using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.DB
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        //AppUser will have a table name of Users
        public DbSet<AppUser> Users { get; set; }

        //UserLike will have a table name of Likes
        public DbSet<UserLike> Likes { get; set; }

        //give entities some configuration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //user like configuration
            //key is combination of sourceUserId and LikedUserId
            builder.Entity<UserLike>()
                    .HasKey(k => new { k.SourceUserId, k.LikedUserId });
            //build relationships between AppUser and UserLike. Here the users liked by the logged in user
            builder.Entity<UserLike>()
                    .HasOne(s => s.SourceUser)
                    .WithMany(l => l.UsersILiked)
                    .HasForeignKey(s => s.SourceUserId)
                    .OnDelete(DeleteBehavior.Cascade) //when the user is deleted then delete the related entities. For sql server use DeleteBehavior.NoAction 
            ;
            //build relationships between AppUser and UserLike. Here the logged in user liked by other users
            builder.Entity<UserLike>()
                    .HasOne(s => s.LikedUser)
                    .WithMany(l => l.UsersLikedMe)
                    .HasForeignKey(s => s.LikedUserId)
                    .OnDelete(DeleteBehavior.Cascade) //when the user is deleted then delete the related entities. For sql server use DeleteBehavior.NoAction 
            ;

        }
    }
}