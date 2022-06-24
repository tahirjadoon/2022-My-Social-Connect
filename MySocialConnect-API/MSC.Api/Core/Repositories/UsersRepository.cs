using System.Collections.Generic;
using System.Linq;
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

    
}