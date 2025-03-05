using AutoMapper;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<CreateServiceDTO, Service>().ReverseMap();
            CreateMap<ServiceInformationDTO, Service>().ReverseMap();
            CreateMap<Service, ServiceInformationDTO>();
        }
    }
}
