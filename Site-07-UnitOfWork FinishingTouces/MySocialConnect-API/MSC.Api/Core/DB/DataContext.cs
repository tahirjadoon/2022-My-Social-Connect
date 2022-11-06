using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.DB
{
    //Removed use of DataContext : DbContext
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
                                                IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
                                                IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }


        //no need to DbSet Users any more with Identity
        //public DbSet<AppUser> Users { get; set; }

        //UserLike will have a table name of Likes
        public DbSet<UserLike> Likes { get; set; }

        //UserMessage will have a table name of Messages
        public DbSet<Message> Messages { get; set; }

        public DbSet<SignalRGroup> SignalRGroups { get; set; }

        public DbSet<SignalRConnection> SignalRConnections { get; set; }

        //give entities some configuration
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //dont foget to add migrations
            //dotnet ef migrations add MessageEntityAdded -o Core/DB/Migrations 
            //and then either issue command "dotnet ef database update"
            //or do dontnet run. For this check program.cs "CUSTOM: Seed Data Start" section

            //Due to use of Identity
            CreateUserRole(builder);

            CreateUserLike(builder);
            CreateMessage(builder);
        }

        /// <summary>
        /// User roles due to use of Identity
        /// </summary>
        /// <param name="builder"></param>
        private void CreateUserRole(ModelBuilder builder)
        {
            builder.Entity<AppUser>()
                    .HasMany(ur => ur.UserRoles)
                    .WithOne(u => u.User)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired()
            ;

            builder.Entity<AppRole>()
                    .HasMany(ur => ur.UserRoles)
                    .WithOne(u => u.Role)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired()
            ;
        }

        /// <summary>
        /// Configure User Likes
        /// </summary>
        /// <param name="builder"></param>
        private void CreateUserLike(ModelBuilder builder)
        {
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

        /// <summary>
        /// Confiure User Messages
        /// </summary>
        /// <param name="builder"></param>
        private void CreateMessage(ModelBuilder builder)
        {
            //user message configuration
            //receiver
            builder.Entity<Message>()
                    .HasOne(r => r.Receipient)
                    .WithMany(m => m.MessagesReceived)
                    .OnDelete(DeleteBehavior.Restrict) //both the parties need to delete the message to be removed from the database
            ;

            //sender
            builder.Entity<Message>()
                    .HasOne(s => s.Sender)
                    .WithMany(m => m.MessagesSent)
                    .OnDelete(DeleteBehavior.Restrict)
            ;
        }
    }
}