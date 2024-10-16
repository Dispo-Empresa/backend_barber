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
        public async Task<Appointment> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await unitOfWork.QueryUnderTransactionAsync(cancellationToken, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = await appointmentRepository.GetAsync(cancellationToken, id);
                if (appointment is null)
                {
                    throw new NotFoundException("Agendamento não existe.");
                }

                return appointment;
            });
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = mapper.Map<Appointment>(createAppointmentDTO);
                await appointmentRepository.AddAsync(cancellationToken, appointment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = await appointmentRepository.GetAsync(cancellationToken, id);
                if (appointment is null)
                {
                    throw new NotFoundException("Agendamento não existe.");
                }

                appointment.AcceptedUserObservation = informAppointmentProblemDTO.Problem;
                appointmentRepository.Update(appointment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });
        }
    }
}
