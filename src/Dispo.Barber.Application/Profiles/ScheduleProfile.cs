using AutoMapper;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Profiles
{
    public class ScheduleProfile : Profile
    {
        public ScheduleProfile()
        {
            CreateMap<UserSchedule, CreateScheduleDTO>().ReverseMap();
        }
    }
}
