using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MSC.Api.Core.Entities;

//IR_REFACOR: derive from IdentityUser and make the key type int
[Index(nameof(GuId))]
[Index(nameof(UserName))]
public class AppUser : IdentityUser<int>
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid GuId { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string DisplayName { get; set; }
    public string Gender { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public ICollection<Photo> Photos { get; set; }
    public DateTime LastActive { get; set; } = DateTime.Now;
    public DateTime CreatedOn { get; set; } = DateTime.Now;
    public DateTime UpdatedOn { get; set; } = DateTime.Now;

    /// <summary>
    /// The users that have liked logged in user. CheckDB Context for relationships
    /// </summary>
    public ICollection<UserLike> UsersLikedMe { get; set; }

    /// <summary>
    /// The users that the logged in user liked. CheckDB Context for relationships
    /// </summary>
    public ICollection<UserLike> UsersILiked { get; set; }

    public ICollection<Message> MessagesSent { get; set; }
    public ICollection<Message> MessagesReceived { get; set; }

    /// <summary>
    /// Added due to identity, acting as a join table
    /// </summary>
    public ICollection<AppUserRole> UserRoles { get; set; }
}