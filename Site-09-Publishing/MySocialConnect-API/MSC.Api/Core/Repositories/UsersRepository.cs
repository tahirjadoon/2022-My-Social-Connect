using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.Constants;
using MSC.Api.Core.DB;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Dto.Helpers;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;

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

    public async Task<PageList<UserDto>> GetUsersAsync(UserParams userParams)
    {
        //query is IQueryable
        var query = _context.Users.AsQueryable();

        //apply filters
        query = query.Where(u => u.GuId != userParams.CurrentUserGuid.Value);
        query = query.Where(u => u.Gender == userParams.Gender);

        var minDob = userParams.MaxAge.CalculateMinDob();
        var maxDob = userParams.MinAge.CalculateMaxDob();

        query = query.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);

        if (!string.IsNullOrWhiteSpace(userParams.OrderBy))
        {
            //the new switch statement. _ is the default
            query = userParams.OrderBy switch
            {
                DataConstants.Created => query.OrderByDescending(u => u.CreatedOn),
                _ => query.OrderByDescending(u => u.LastActive)
            };
        }

        //projectTo to get the photos 
        var finalQuery = query
                        .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                        .AsNoTracking();

        //page list has the static method that receive the IQueryable so use it and will return the object
        var pageList = await PageList<UserDto>.CreateAsync(finalQuery, userParams.PageNumber, userParams.PageSize);
        return pageList;
    }

    public async Task<UserDto> GetUserByGuidAsync(Guid id, bool isCurrentUser)
    {
        //var user = await _context.Users.FindAsync(id);
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Id == id);
        //return user;

        //using automapper queryable extensions
        /*
        var user = await _context.Users
                    .Where(x => x.GuId == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
        */

        //ignore query filter for the current user as it is setup via dbcontext
        var query = _context.Users
                    .Where(x => x.GuId == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
        if (isCurrentUser)
            query = query.IgnoreQueryFilters();

        var user = await query.FirstOrDefaultAsync();
        return user;
    }

    public async Task<UserDto> GetUserAsync(int id, bool isCurrentUser)
    {
        //var user = await _context.Users.FindAsync(id);
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Id == id);
        //return user;

        //using automapper queryable extensions
        /*
        var user = await _context.Users
                    .Where(x => x.Id == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
        */

        //ignore query filter for the current user as it is setup via dbcontext
        var query = _context.Users
                    .Where(x => x.Id == id)
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
        if (isCurrentUser)
            query = query.IgnoreQueryFilters();

        var user = await query.FirstOrDefaultAsync();
        return user;
    }

    public async Task<UserDto> GetUserAsync(string userName, bool isCurrentUser)
    {
        if (userName == null)
            throw new ValidationException("Invalid userName");
        //add photos as eager loading
        //var user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.UserName.ToLower() == userName.ToLower());
        //return user;

        //using automapper queryable extensions
        /*
        var user = await _context.Users
                    .Where(x => x.UserName.ToLower() == userName.ToLower())
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsSplitQuery()
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
        return user;
        */

        //ignore query filter for the current user as it is setup via dbcontext
        var query = _context.Users
                    .Where(x => x.UserName.ToLower() == userName.ToLower())
                    .ProjectTo<UserDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();
        if (isCurrentUser)
            query = query.IgnoreQueryFilters();

        var user = await query.FirstOrDefaultAsync();
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

    public async Task<AppUser> GetAppUserAsync(int id, bool includePhotos = false)
    {
        if (id <= 0)
            throw new ValidationException("Invalid userName");
        AppUser user = null;
        if (!includePhotos)
            user = await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
        else
            user = await _context.Users.Include(p => p.Photos).SingleOrDefaultAsync(x => x.Id == id);
        return user;
    }

    public void Register(AppUser appUser)
    {
        if (appUser == null)
            throw new ValidationException("Invalid user");

        _context.Users.Add(appUser);
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

    /*
    public async Task<bool> SaveAllAsync()
    {
        //make sure that the changes have been saved
        var isSave = await _context.SaveChangesAsync() > 0;
        return isSave;
    }
    */

    public async Task<AppUser> GetUserByPhotoId(int photoId)
    {
        var user = await _context.Users
                                .Include(p => p.Photos)
                                .IgnoreQueryFilters()
                                .Where(p => p.Photos.Any(x => x.Id == photoId))
                                .FirstOrDefaultAsync();
        return user;
    }
}