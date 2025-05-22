using AutoMapper;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, User>().ReverseMap();
            CreateMap<UserInformationDTO, User>().ReverseMap();
            CreateMap<CreateOwnerUserDTO, User>();
            CreateMap<CreateEmployeeUserDTO, User>();
            CreateMap<FinalizeEmployeeUserDTO, User>();
            CreateMap<ServiceUser, ServiceInformationDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Service.Id))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Service.Description))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Service.Price))
                .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Service.Duration))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Service.Status))
                .ForMember(dest => dest.Enabled, opt => opt.MapFrom(src => true));

        }
    }
}
