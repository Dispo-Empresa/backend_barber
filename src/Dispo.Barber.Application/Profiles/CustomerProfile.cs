using AutoMapper;
using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Application.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<CustomerDTO, Customer>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => StringUtils.FormatPhoneNumber(src.Phone)))
                .ReverseMap()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => StringUtils.FormatPhoneNumber(src.Phone)));

            CreateMap<CustomerDetailDTO, Customer>()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => StringUtils.FormatPhoneNumber(src.Phone)))
                .ReverseMap()
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => StringUtils.FormatPhoneNumber(src.Phone)));
        }
    }
}
