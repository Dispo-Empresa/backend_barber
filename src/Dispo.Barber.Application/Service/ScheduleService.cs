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

        public async Task DeleteAsync(CancellationToken cancellationToken, long scheduleId)
        {
            var schedule = await repository.GetAsync(cancellationToken, scheduleId) ?? throw new NotFoundException("Horário não encontrado.");
            repository.Delete(schedule);
            await repository.SaveChangesAsync(cancellationToken);
        }
    }
}
