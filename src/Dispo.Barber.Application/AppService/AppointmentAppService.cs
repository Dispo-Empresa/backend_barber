using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService
{
    public class AppointmentAppService(IUnitOfWork unitOfWork, IMapper mapper) : IAppointmentAppService
    {
        public async Task CreateAsync(CreateAppointmentDTO createAppointmentDTO)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = mapper.Map<Appointment>(createAppointmentDTO);
                await appointmentRepository.AddAsync(appointment);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }
    }
}
