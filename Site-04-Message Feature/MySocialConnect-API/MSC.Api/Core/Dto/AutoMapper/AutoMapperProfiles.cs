using System;
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
        Map_UserUpdate_To_AppUser();
        Map_UserRegister_To_AppUser();
        Map_AppUser_To_UserTokenDto();
        Map_Message_to_MessageDto();
    }

    #region Mappers

    private void Map_AppUser_To_UserDto()
    {
        //same name propertirs will be automatically mapped
        //Age will also get automatically mapped since source has GetAge, the keywor Age are the same
        //PhotoUrl we'll need to map manually. will pick the url where isMain is true. Do check for null. 
        //  ***Hint: An expression tree lambda may not contain a null propagating operator.
        //  so use a function intead
        CreateMap<AppUser, UserDto>()
        //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToTitleCase()))
        .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Photos)))
        .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
    }

    private void Map_Photo_To_PhotoDto()
    {
        CreateMap<Photo, PhotoDto>();
    }

    private void Map_UserUpdate_To_AppUser()
    {
        CreateMap<UserUpdateDto, AppUser>();
    }

    private void Map_UserRegister_To_AppUser()
    {
        CreateMap<UserRegisterDto, AppUser>()
        .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLowerInvariant()))
        .ForMember(dest => dest.GuId, opt => opt.MapFrom(src => Guid.NewGuid()))
        ;
    }

    private void Map_AppUser_To_UserTokenDto()
    {
        CreateMap<AppUser, UserTokenDto>()
        .ForMember(dest => dest.MainPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Photos)))
        ;
    }

    private void Map_Message_to_MessageDto()
    {
        CreateMap<Message, MessageDto>()
        .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Sender.Photos)))
        .ForMember(dest => dest.ReceipientPhotoUrl, opt => opt.MapFrom(src => PickMainUrl(src.Receipient.Photos)))
        .ForMember(dest => dest.SenderGuid, opt => opt.MapFrom(src => src.Sender.GuId))
        .ForMember(dest => dest.ReceipientGuid, opt => opt.MapFrom(src => src.Receipient.GuId))
        ;
    }

    #endregion Mappers

    #region Helper Functions

    //converted to static method after conversion to using automapper queryable extensions
    private static string PickMainUrl(ICollection<Photo> photos)
    {
        if (photos == null || !photos.Any()) return string.Empty;
        var url = photos.FirstOrDefault(x => x.IsMain)?.Url ?? string.Empty;
        return url;
    }

    #endregion Helper Functions

}