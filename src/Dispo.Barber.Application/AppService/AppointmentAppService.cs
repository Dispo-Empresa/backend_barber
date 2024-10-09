using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Exception;

namespace Dispo.Barber.Application.AppService
{
    public class AppointmentAppService(IUnitOfWork unitOfWork, IMapper mapper) : IAppointmentAppService
    {
        public async Task<Appointment> GetAsync(long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            return await unitOfWork.QueryUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment =  await appointmentRepository.GetAsync(id);
                if (appointment is null)
                {
                    throw new NotFoundException("Agendamento não existe.");
                }

                return appointment;
            });
        }

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

        public async Task InformProblemAsync(long id, InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationTokenSource.Token, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = await appointmentRepository.GetAsync(id);
                if (appointment is null)
                {
                    throw new KeyNotFoundException();
                }

                appointment.AcceptedUserObservation = informAppointmentProblemDTO.Problem;
                appointmentRepository.Update(appointment);
                await unitOfWork.SaveChangesAsync(cancellationTokenSource.Token);
            });
        }
    }
}
