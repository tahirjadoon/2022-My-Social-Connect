using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext _context;

    public UsersRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AppUser>> GetUsers()
    {
        var users = await _context.Users.ToListAsync();
        return users;
    }

    public async Task<AppUser> GetUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user;
    }

    public async Task<AppUser> GetUser(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        return user;
    }

    public async Task<AppUser> Register(AppUser user)
    {
        if(user == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        
        return user;
    }

    public async Task<bool> UserExists(string userName)
    {
        if(userName == null)
            throw new ValidationException("Invalid userName");

        var isUser = await _context.Users.AnyAsync(x => x.UserName.ToLower() == userName.ToLower());
        return isUser;
    }

    
}