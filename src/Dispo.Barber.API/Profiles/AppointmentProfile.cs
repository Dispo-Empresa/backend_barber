using AutoMapper;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class AppointmentProfile : Profile
    {
        public AppointmentProfile()
        {
            CreateMap<CreateAppointmentDTO, Appointment>()
                .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services == null ? new List<ServiceAppointment>() : src.Services.Select(service => new ServiceAppointment
                {
                    ServiceId = service,
                })))
                .ReverseMap();
        }
    }
}
