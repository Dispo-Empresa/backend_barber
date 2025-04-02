using AutoMapper;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Google.Api.Gax;

namespace Dispo.Barber.Domain.Services
{
    public class ScheduleService(IMapper mapper, IScheduleRepository repository) : IScheduleService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO)
        {
            var schedule = mapper.Map<UserSchedule>(createScheduleDTO);

            await ValidateSchedule(cancellationToken, schedule);

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

        private async Task ValidateSchedule(CancellationToken cancellationToken, UserSchedule schedule)
        {
            if (schedule.DayOff && schedule.StartDay > schedule.EndDay)
            {
                throw new BusinessException("O horário inicial não pode ser maior nem igual o horário final.");
            }

            if (!schedule.IsDatesValid())
            {
                await ValidateOverride(cancellationToken, schedule);
            }

            var (start, end) = schedule.ParseDates();
            if (schedule.IsRest && (start > end || start == end))
            {
                throw new BusinessException("O horário inicial não pode ser maior nem igual o horário final.");
            }

            await ValidateOverride(cancellationToken, schedule);
        }

        private async Task ValidateOverride(CancellationToken cancellationToken, UserSchedule schedule)
        {
            var existingSchedules = await repository.GetAsync(cancellationToken, w => w.Enabled && (w.IsRest || w.DayOff) && w.UserId == schedule.UserId && w.DayOfWeek == schedule.DayOfWeek);
            foreach (var existingSchedule in existingSchedules)
            {
                if (existingSchedule.DayOff)
                {
                    HandleDayOffOverride(schedule, existingSchedule);
                    continue;
                }

                HandleRestOverride(schedule, existingSchedule);
            }
        }

        private void HandleDayOffOverride(UserSchedule schedule, UserSchedule existingSchedule)
        {
            if (schedule.StartDay < existingSchedule.EndDay && schedule.EndDay > existingSchedule.StartDay)
            {
                throw new BusinessException("O horário novo sobrepõe um já existente.");
            }
        }

        private void HandleRestOverride(UserSchedule schedule, UserSchedule existingSchedule)
        {
            if (!existingSchedule.IsDatesValid() || !schedule.IsDatesValid())
            {
                return;
            }

            var (newStart, newEnd) = schedule.ParseDates();
            var (existingStart, existingEnd) = existingSchedule.ParseDates();
            if (newStart < existingEnd && newEnd > existingStart)
            {
                throw new BusinessException("O horário novo sobrepõe um já existente.");
            }
        }
    }
}
