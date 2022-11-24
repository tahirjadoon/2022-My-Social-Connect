using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MSC.Api.Core.DB;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.Repositories;

public class PhotoRepository : IPhotoRepository
{
    private readonly DataContext _context;

    public PhotoRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        var photos = await _context.Photos
                            .IgnoreQueryFilters()
                            .Where(p => p.IsApproved == false)
                            .Select(u => new PhotoForApprovalDto
                            {
                                Id = u.Id,
                                Username = u.AppUser.UserName,
                                UserGuid = u.AppUser.GuId,
                                UserId = u.AppUser.Id,
                                Url = u.Url,
                                IsApproved = u.IsApproved
                            }).ToListAsync();
        return photos;
    }

    public async Task<Photo> GetPhotoById(int id)
    {
        var photo = await _context.Photos
                            .IgnoreQueryFilters()
                            .SingleOrDefaultAsync(x => x.Id == id);
        return photo;
    }

    public void RemovePhoto(Photo photo)
    {
        _context.Photos.Remove(photo);
    }
}
