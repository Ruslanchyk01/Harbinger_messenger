using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs;
using Api.Models;
using AutoMapper;
using Api.Extensions;


namespace Api.Helpers
{
    public class AutoMapperProfiles : Profile
     {
         public AutoMapperProfiles()
         {
             CreateMap<AppUser, MemberDTO>()
                 .ForMember(dest => dest.PhotoUrl, 
                     opt => opt.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                 .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalcuateAge()));
             CreateMap<Photo, PhotoDTO>();
             CreateMap<MemberUpdateDTO, AppUser>();
         }
     }
}