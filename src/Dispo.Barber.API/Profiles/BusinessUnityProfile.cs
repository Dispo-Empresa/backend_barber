using AutoMapper;
using Dispo.Barber.Domain.DTO.BusinessUnity;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class BusinessUnityProfile : Profile
    {
        public BusinessUnityProfile()
        {
            CreateMap<CreateBusinessUnityDTO, BusinessUnity>().ReverseMap();
        }
    }
}
