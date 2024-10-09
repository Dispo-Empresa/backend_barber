using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.AppService
{
    public class ScheduleAppService(IUnitOfWork unitOfWork, IMapper mapper) : IScheduleAppService
    {
        public async Task CreateAsync(CancellationToken cancellationToken, CreateScheduleDTO createScheduleDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var scheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
                var schedule = mapper.Map<UserSchedule>(createScheduleDTO);
                await scheduleRepository.AddAsync(schedule);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task DeleteAsync(CancellationToken cancellationToken, long scheduleId)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var scheduleRepository = unitOfWork.GetRepository<IScheduleRepository>();
                var schedule = await scheduleRepository.GetAsync(scheduleId) ?? throw new NotFoundException("Horário não encontrado.");
                scheduleRepository.Delete(schedule);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }
    }
}
