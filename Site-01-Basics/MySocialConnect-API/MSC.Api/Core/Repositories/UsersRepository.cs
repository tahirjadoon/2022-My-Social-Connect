using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;

    public UsersRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        //var users = await _context.Users.ToListAsync();
        //add photos as eager loading
        //var users = await _context.Users.Include(p => p.Photos).ToListAsync();
        //return users;

        //using automapper queryable extensions
        var users = await _context.Users
                            .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                            .AsSplitQuery()
                            .AsNoTracking()
                            .ToListAsync();
        return users;
    }

    public async Task<UserDto> GetUserByGuidAsync(Guid id)
    {
        //var user = await _context.Users.FindAsync(id);
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Id == id);
        //return user;

        //using automapper queryable extensions
        var user = await _context.Users
                    .Where(x => x.GuId == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
    }

    public async Task<UserDto> GetUserAsync(int id)
    {
        //var user = await _context.Users.FindAsync(id);
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Id == id);
        //return user;

        //using automapper queryable extensions
        var user = await _context.Users
                    .Where(x => x.Id == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
    }

    public async Task<UserDto> GetUserAsync(string userName)
    {
        if (userName == null)
            throw new ValidationException("Invalid userName");
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        //return user;

        //using automapper queryable extensions
        var user = await _context.Users
                    .Where(x => x.UserName.ToLower() == userName.ToLower())
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
    }

    public async Task<AppUser> GetAppUserAsync(string userName, bool includePhotos = false)
    {
        if (userName == null)
            throw new ValidationException("Invalid userName");
        AppUser user = null;
        if (!includePhotos)
            user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        else
            user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        return user;
    }

    public async Task<bool> RegisterAsync(AppUser appUser)
    {
        if (appUser == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(appUser);
        var isSave = await SaveAllAsync();
        return isSave;
    }

    public async Task<bool> UserExistsAsync(string userName)
    {
        var user = await GetAppUserAsync(userName);
        return user != null;
    }

    //marking the entity only that it has been modified
    public void Update(AppUser user)
    {
        if (user == null)
            throw new ValidationException("Invalid user");

        //ef adds a flag to the entity that it has been modified
        _context.Entry<AppUser>(user).State = EntityState.Modified;
    }

    public async Task<bool> SaveAllAsync()
    {
        //make sure that the changes have been saved
        var isSave = await _context.SaveChangesAsync() > 0;
        return isSave;
    }
}