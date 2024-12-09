using AutoMapper;
using Dispo.Barber.Application.Repository;
using Dispo.Barber.Application.Service.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enum;
using Dispo.Barber.Domain.Exception;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Application.Service
{
    public class AppointmentService(IMapper mapper, IAppointmentRepository repository, ICustomerRepository customerRepository) : IAppointmentService
    {
        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id)
        {
            var appointment = await repository.GetAsync(cancellationToken, id);

            if (appointment is null)
            {
                throw new NotFoundException("Agendamento não existe.");
            }

            appointment.Status = AppointmentStatus.Canceled;
            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO)
        {
            var appointment = mapper.Map<Appointment>(createAppointmentDTO);
            var existingCustomer = await customerRepository.GetAsync(cancellationToken, createAppointmentDTO.Customer.Id.Value);
            if (existingCustomer != null)
            {
                appointment.Customer = null;
                appointment.CustomerId = existingCustomer.Id;
            }
            else
            {
                appointment.Customer.Phone = StringUtils.FormatPhoneNumber(appointment.Customer.Phone);
            }

            appointment.Status = AppointmentStatus.Scheduled;
            await repository.AddAsync(cancellationToken, appointment);
            await repository.SaveChangesAsync(cancellationToken);
            //await smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateAppointmentMessage(appointment), MessageType.Sms);
        }

        public async Task<Appointment> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Agendamento não existe.");
        }

        public async Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            var appointment = await repository.GetAsync(cancellationToken, id);
            if (appointment is null)
            {
                throw new NotFoundException("Agendamento não existe.");
            }

            appointment.AcceptedUserObservation = informAppointmentProblemDTO.Problem;
            appointment.Status = AppointmentStatus.Canceled;
            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CancelAllTodayAsync(CancellationToken cancellationToken, long userId)
        {
            // TODO: Notificar clientes que tiveram os agendamentos cancelados.
            await repository.CancelAllTodayAsync(cancellationToken, userId);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId)
        {
            return await repository.GetNextAppointmentsAsync(cancellationToken, userId);
        }

        //private async Task SendNotificationBySMS(Appointment appointment)
        //{
        //    smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateAppointmentMessage(appointment), MessageType.Sms);
        //}
    }
}
