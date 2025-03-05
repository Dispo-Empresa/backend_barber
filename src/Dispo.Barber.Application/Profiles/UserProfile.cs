using AutoMapper;
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
        }
    }
}
