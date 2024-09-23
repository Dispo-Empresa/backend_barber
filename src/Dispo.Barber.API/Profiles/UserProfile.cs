using AutoMapper;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, User>().ReverseMap();
        }
    }
}
