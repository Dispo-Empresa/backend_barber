using AutoMapper;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CreateServiceDTO, Service>().ReverseMap();
            CreateMap<ServiceInformationDTO, Service>().ReverseMap();
        }
    }
}
