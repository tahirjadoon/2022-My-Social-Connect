using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using MSC.Api.Core.Entities;
using MSC.Api.Core.Extensions;

namespace MSC.Api.Core.Dto.AutoMapper;
public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {
        Map_AppUser_To_UserDto();
        Map_Photo_To_PhotoDto();
    }

#region Mappers

    private void Map_AppUser_To_UserDto()
    {
        //same nme propertirs will be automatically mapped
        //Age will also get automatically mapped since source has GetAge, the keywor Age are the same
        //PhotoUrl we'll need to map manually. will pick the url where isMain is true. Do check for null. 
        //  ***Hint: An expression tree lambda may not contain a null propagating operator.
        //  so use a function intead
        CreateMap<AppUser, UserDto>()
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => PickMainUrl_AppUser_To_UserDto(src.Photos)))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
    }

    private void Map_Photo_To_PhotoDto()
    {
        CreateMap<Photo, PhotoDto>();
    }

#endregion Mappers

#region Helper Functions

    //converted to static method after conversion to using automapper queryable extensions
    private static string PickMainUrl_AppUser_To_UserDto(ICollection<Photo> photos)
    {
        if (photos == null || !photos.Any()) return string.Empty;
        var url = photos.FirstOrDefault(x => x.IsMain)?.Url ?? string.Empty;
        return url;
    }

#endregion Helper Functions

}