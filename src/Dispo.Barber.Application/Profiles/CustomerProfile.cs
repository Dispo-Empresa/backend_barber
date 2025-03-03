using AutoMapper;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerDTO, Customer>().ReverseMap();
            CreateMap<CustomerDetailDTO, Customer>().ReverseMap();
        }
    }
}
