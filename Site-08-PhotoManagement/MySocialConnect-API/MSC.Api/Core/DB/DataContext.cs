using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.DB;

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

    public DbSet<Photo> Photos { get; set; }

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

        //add a Query filter to only return approved photos
        builder.Entity<Photo>().HasQueryFilter(p => p.IsApproved);

        //keep this at the bottom. UTC date fix per EF github 
        builder.ApplyUtcDateTimeConverter();
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

#region UTC datetime fix from EF Core github issue

/// <summary>
/// Will convert the date times to UTC
/// </summary>
public static class UtcDateAnnotation
{
    private const String IsUtcAnnotation = "IsUtc";
    private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
      new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

    private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
      new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

    public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
      builder.HasAnnotation(IsUtcAnnotation, isUtc);

    public static Boolean IsUtc(this IMutableProperty property) =>
      ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

    /// <summary>
    /// Make sure this is called after configuring all your entities.
    /// </summary>
    public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
    {
        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (!property.IsUtc())
                {
                    continue;
                }

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(UtcConverter);
                }

                if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(UtcNullableConverter);
                }
            }
        }
    }
}

#endregion UTC datetime