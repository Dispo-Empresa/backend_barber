﻿using AutoMapper;
using Dispo.Barber.Domain.DTOs.Company;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<CreateCompanyDTO, Company>()
            .ForMember(dest => dest.ServicesCompany, opt => opt.MapFrom(src => src.Services == null ? new List<ServiceCompany>() : src.Services.Select(service => new ServiceCompany
            {
                Service = new Service
                {
                    Description = service.Description,
                    Price = service.Price,
                    Duration = service.Duration
                }
            })))
            .ReverseMap();
        }
    }
}
