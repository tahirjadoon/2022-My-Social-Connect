using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Constants;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.DB;
public class Seed
{
    //UserManager used instead of DataContext
    public static async Task SeedUsers(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        //if we have users in the table then do not do any thing
        if (await userManager.Users.AnyAsync()) return;

        //File location
        var file = "Core/DB/UserSeedData.json";

        //check file exists
        var isFile = await Task.Run(() => File.Exists(file));
        if (!isFile) return;

        //read file
        var userData = await File.ReadAllTextAsync(file);
        //make sure that we have user data
        if (string.IsNullOrWhiteSpace(userData)) return;

        //get object from json
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
        //check users
        if (users == null || !users.Any()) return;

        //Create roles 
        foreach (var role in SiteIdentityConstants.SiteRoles)
        {
            await roleManager.CreateAsync(role);
        }

        //add password to the users, make username lower case and track users
        foreach (var user in users)
        {
            user.UserName = user.UserName.ToLowerInvariant();
            user.CreatedOn = DateTime.SpecifyKind(user.CreatedOn, DateTimeKind.Utc);
            user.UpdatedOn = DateTime.SpecifyKind(user.UpdatedOn, DateTimeKind.Utc);
            user.LastActive = DateTime.SpecifyKind(user.LastActive, DateTimeKind.Utc);
            //make the photo IsApproved
            user.Photos.First().IsApproved = true;
            //saves the users to the database as well. no need to do SaveChangesAsync
            await userManager.CreateAsync(user, "A1abcd");
            //assign a role to the user as well
            await userManager.AddToRoleAsync(user, SiteIdentityConstants.Role_Member);
        }

        //create a new admin user so that we have atleast one user to begin with 
        var admin = new AppUser() { UserName = "admin", DisplayName = "Admin", DateOfBirth = DateTime.Now.AddYears(-19), GuId = System.Guid.NewGuid(), Gender = "male" };
        await userManager.CreateAsync(admin, "A1abcd");
        //add assign Admin and Moderator roles to Admin user
        await userManager.AddToRolesAsync(admin, new[] { SiteIdentityConstants.Role_Admin, SiteIdentityConstants.Role_Moderator });
    }
}
