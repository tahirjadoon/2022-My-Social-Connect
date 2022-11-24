using System.Collections.Generic;
using System.Threading.Tasks;
using MSC.Api.Core.Dto;
using MSC.Api.Core.Entities;

namespace MSC.Api.Core.BusinessLogic;
public interface IPhotoBusinessLogic
{
    Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
    Task<Photo> GetPhotoById(int id);
    Task<BusinessResponse> ApprovePhoto(int photoId);
    Task<BusinessResponse> RemovePhoto(int photoId);
}
