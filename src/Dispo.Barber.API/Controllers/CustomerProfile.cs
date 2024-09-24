using AutoMapper;
using Dispo.Barber.Domain.DTO;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Controllers
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerDTO, Customer>().ReverseMap();
        }
    }
}
