using AutoMapper;
using BackEnd.DTOS;
using BackEnd.Entities;
using System.Collections.Generic;
using System.Linq;

namespace BackEnd.Helpers
{
    public class AutoMapperProfile:Profile
    {
       public AutoMapperProfile()
        {
            CreateMap<AppUser, MemberDto>()
                    .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<Photo, PhotoDto>();
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<RegisterDTO,AppUser>();

            CreateMap<Message, MessageDto>()
                    .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(l => l.IsMain).Url))
                    .ForMember(dest => dest.SenderPhtoUrl, opt => opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(l => l.IsMain).Url));

        }
    }
}
