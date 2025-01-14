using AutoMapper;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.API.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<UserSchedule, CreateScheduleDTO>().ReverseMap();
        }
    }
}
