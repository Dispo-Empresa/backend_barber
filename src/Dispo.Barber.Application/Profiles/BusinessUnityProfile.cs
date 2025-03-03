using AutoMapper;
using Dispo.Barber.Domain.DTOs.BusinessUnity;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class BusinessUnityProfile : Profile
    {
        public BusinessUnityProfile()
        {
            CreateMap<CreateBusinessUnityDTO, BusinessUnity>().ReverseMap();
        }
    }
}
