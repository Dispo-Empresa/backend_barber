using AutoMapper;
using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Exception;
using Dispo.Barber.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace Dispo.Barber.Application.AppService
{
    public class AppointmentAppService(IUnitOfWork unitOfWork, IMapper mapper, ISmsService smsService) : IAppointmentAppService
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
            try
            {
                await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
                {
                    var appointment = mapper.Map<Appointment>(createAppointmentDTO);
                    var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                    var existingCustomer = await unitOfWork.GetRepository<ICustomerRepository>().GetAsync(cancellationToken, createAppointmentDTO.Customer.Id.Value);
                    if (existingCustomer != null)
                    {
                        appointment.Customer = null;
                        appointment.CustomerId = existingCustomer.Id;
                    }

                    appointment.Status = AppointmentStatus.Scheduled;
                    await appointmentRepository.AddAsync(cancellationToken, appointment);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    //await smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateAppointmentMessage(appointment), MessageType.Sms);
                });
            }
            catch (Exception e)
            {
                throw;
            }
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

        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            await unitOfWork.ExecuteUnderTransactionAsync(cancellationToken, async () =>
            {
                var appointmentRepository = unitOfWork.GetRepository<IAppointmentRepository>();
                var appointment = await appointmentRepository.GetAsync(cancellationToken, id);
                appointment.Status = AppointmentStatus.Canceled;
                await appointmentRepository.AddAsync(cancellationToken, appointment);
                await unitOfWork.SaveChangesAsync(cancellationToken);
            });

        }

        private async Task SendNotificationBySMS(Appointment appointment)
        {
            smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateAppointmentMessage(appointment), MessageType.Sms);
        }
    }
}
