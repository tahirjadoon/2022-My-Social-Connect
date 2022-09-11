using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Core.DB;
public class Seed
{
    public static async Task SeedUsers(DataContext context)
    {
        //if we have users in the table then do not do any thing
        if(await context.Users.AnyAsync()) return;
        
        //File location
        var file = "Core/DB/UserSeedData.json";
        
        //check file exists
        var isFile = await Task.Run(() => File.Exists(file));
        if(!isFile) return;
        
        //read file
        var userData = await File.ReadAllTextAsync(file);
        //make sure that we have user data
        if(string.IsNullOrWhiteSpace(userData)) return;
        
        //get object from json
        var users = JsonSerializer.Deserialize<List<AppUser>>(userData);
        //check users
        if(users == null || !users.Any()) return;

        //all the users will get the same password so get it here outside the loop
        var hashKey = "password".ComputeHashHmacSha512();
        if(hashKey == null) return;

        //add password to the users, make username lower case and track users
        foreach(var user in users)
        {
            user.UserName = user.UserName.ToLowerInvariant();
            user.PasswordHash = hashKey.Hash;
            user.PasswordSalt = hashKey.Salt;

            //we are only adding tracking to the user, save changes will happen outside of the loop
            context.Users.Add(user);
        }

        //add to the database
        await context.SaveChangesAsync();
    }
}
