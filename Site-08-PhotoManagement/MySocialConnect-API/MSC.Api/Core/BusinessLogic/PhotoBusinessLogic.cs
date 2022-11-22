using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MSC.Api.Core.DB.UnitOfWork;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Services;

namespace MSC.Api.Core.BusinessLogic;

public class PhotoBusinessLogic : IPhotoBusinessLogic
{
    private readonly IUnitOfWork _uow;
    private readonly IPhotoService _photoService;

    public PhotoBusinessLogic(IUnitOfWork uow, IPhotoService photoService)
    {
        _uow = uow;
        _photoService = photoService;
    }

    public async Task<Photo> GetPhotoById(int id)
    {
        return await _uow.PhotoRepo.GetPhotoById(id);
    }

    public async Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos()
    {
        return await _uow.PhotoRepo.GetUnapprovedPhotos();
    }

    public async Task<BusinessResponse> ApprovePhoto(int photoId)
    {
        var photo = await GetPhotoById(photoId);
        if (photo == null) return new BusinessResponse(HttpStatusCode.NotFound, "Photo not found");
        photo.IsApproved = true;
        //get the user by the photo and if the user has no main photo applied then make the photo being approved as main
        var user = await _uow.UsersRepo.GetUserByPhotoId(photoId);
        if (user != null && !user.Photos.Any(x => x.IsMain))
            photo.IsMain = true;
        if (await _uow.Complete())
            return new BusinessResponse(HttpStatusCode.OK, "Photo Approved");
        return new BusinessResponse(HttpStatusCode.BadRequest, "Something went bad and could not approve photo");
    }

    public async Task<BusinessResponse> RemovePhoto(int photoId)
    {
        var photo = await GetPhotoById(photoId);
        if (photo == null) return new BusinessResponse(HttpStatusCode.NotFound, "Photo not found");
        if (photo.PublicId != null)
        {
            //cloudinary
            var result = await _photoService.DeletePhotoAync(photo.PublicId);
            if (result.Result == "ok")
                _uow.PhotoRepo.RemovePhoto(photo);
        }
        else
        {
            _uow.PhotoRepo.RemovePhoto(photo);
        }
        if (await _uow.Complete())
            return new BusinessResponse(HttpStatusCode.OK, "Photo Removed");
        return new BusinessResponse(HttpStatusCode.BadRequest, "Something went bad and could not remove photo");
    }
}
