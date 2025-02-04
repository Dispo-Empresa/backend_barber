using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.Service
{
    public class ScheduleService(IMapper mapper, IScheduleRepository repository) : IScheduleService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO)
        {
            var schedule = mapper.Map<UserSchedule>(createScheduleDTO);
            await repository.AddAsync(cancellationToken, schedule);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteAsync(CancellationToken cancellationToken, long id)
        {
            var schedule = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Horário não encontrado.");
            repository.Delete(schedule);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(CancellationToken cancellationToken, long id, UpdateScheduleDTO updateScheduleDTO)
        {
            var schedule = await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Horário não encontrado.");

            if (updateScheduleDTO.DayOfWeek.HasValue && schedule.DayOfWeek != updateScheduleDTO.DayOfWeek.Value)
            {
                schedule.DayOfWeek = updateScheduleDTO.DayOfWeek.Value;
            }

            if (!string.IsNullOrEmpty(updateScheduleDTO.StartDate) && schedule.StartDate != updateScheduleDTO.StartDate)
            {
                schedule.StartDate = updateScheduleDTO.StartDate;
            }

            if (!string.IsNullOrEmpty(updateScheduleDTO.EndDate) && schedule.EndDate != updateScheduleDTO.EndDate)
            {
                schedule.EndDate = updateScheduleDTO.EndDate;
            }

            if (updateScheduleDTO.StartDay.HasValue && schedule.StartDay != updateScheduleDTO.StartDay.Value)
            {
                schedule.StartDay = updateScheduleDTO.StartDay.Value;
            }

            if (updateScheduleDTO.EndDay.HasValue && schedule.EndDay != updateScheduleDTO.EndDay.Value)
            {
                schedule.EndDay = updateScheduleDTO.EndDay.Value;
            }

            if (updateScheduleDTO.IsRest.HasValue && schedule.IsRest != updateScheduleDTO.IsRest.Value)
            {
                schedule.IsRest = updateScheduleDTO.IsRest.Value;
            }

            if (updateScheduleDTO.DayOff.HasValue && schedule.DayOff != updateScheduleDTO.DayOff.Value)
            {
                schedule.DayOff = updateScheduleDTO.DayOff.Value;
            }

            if (updateScheduleDTO.Enabled.HasValue && schedule.Enabled != updateScheduleDTO.Enabled.Value)
            {
                schedule.Enabled = updateScheduleDTO.Enabled.Value;
            }

            repository.Update(schedule);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
