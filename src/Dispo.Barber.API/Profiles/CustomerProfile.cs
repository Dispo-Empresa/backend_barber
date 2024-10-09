using AutoMapper;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerDTO, Customer>().ReverseMap();
        }
    }
}
