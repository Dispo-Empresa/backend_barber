using AutoMapper;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interface;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Domain.Services
{
    public class AppointmentService(IMapper mapper, 
                                    IAppointmentRepository repository, 
                                    ICustomerRepository customerRepository, 
                                    INotificationService notificationService, 
                                    IUserRepository userRepository) : IAppointmentService
    {
        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id, bool notifyUsers = false)
        {
            var appointment = await repository.GetAppointmentByIdAsync(cancellationToken, id);

            if (appointment is null)
            {
                throw new NotFoundException("Agendamento não existe.");
            }

            appointment.Status = AppointmentStatus.Canceled;
            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);
            //await smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateCancelAppointmentMessageSms(appointment), MessageType.Sms);

            if (notifyUsers)
                await SendNotificationByApp(cancellationToken, appointment, "Agendamento Cancelado", notificationService.GenerateCancelAppointmentMessageApp(appointment), NotificationType.CanceledAppointment);
        }

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO, bool notifyUsers = false)
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
                appointment.Customer.Name = appointment.Customer.Name;
                appointment.Customer.Phone = StringUtils.FormatPhoneNumber(appointment.Customer.Phone);
            }

            appointment.Status = AppointmentStatus.Scheduled;
            await repository.AddAsync(cancellationToken, appointment);
            await repository.SaveChangesAsync(cancellationToken);

            //await smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateCreateAppointmentMessageSms(appointment), MessageType.Sms);

            appointment.AcceptedUser = await userRepository.GetAsync(cancellationToken, createAppointmentDTO.AcceptedUserId ?? 0);
            appointment.Customer = await customerRepository.GetAsync(cancellationToken, appointment.CustomerId);

            if (notifyUsers)
                await SendNotificationByApp(cancellationToken, appointment, "Agendamento Confirmado", notificationService.GenerateCreateAppointmentMessageApp(appointment), NotificationType.NewAppointment);
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

        public async Task CancelAllByDateAsync(CancellationToken cancellationToken, long userId, DateTime date)
        {
            // TODO: Notificar clientes que tiveram os agendamentos cancelados.
            await repository.CancelAllByDateAsync(cancellationToken, userId, date);
            var appointmentList = await repository.GetAppointmentByUserAndDateIdSync(cancellationToken, userId, date);
            var notifiedCustomers = new HashSet<long>();

            foreach (var appointment in appointmentList)
            {
                if (!notifiedCustomers.Contains(appointment.CustomerId))
                {
                    //await SendNotificationBySMS(appointment);
                    notifiedCustomers.Add(appointment.CustomerId);
                }
            }
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

        private async Task SendNotificationByApp(CancellationToken cancellationToken, Appointment appointment, string tittle, string body, NotificationType notificationType)
        {
            await notificationService.NotifyAsync(cancellationToken, appointment.AcceptedUser.DeviceToken, tittle, body, notificationType);
        }


        public async Task CancelAllScheduledAsync(CancellationToken cancellationToken, long userId)
        {
            await repository.CancelAllScheduledAsync(cancellationToken, userId);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task CancelAllUserScheduledByDateAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate)
        {
            await repository.CancelAllUserScheduledByDateAsync(cancellationToken, userId, startDate, endDate);
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, DateTime startDate, DateTime endDate)
        {
            return await repository.GetScheduleConflictsAsync(cancellationToken, userId, startDate, endDate);
        }

        public async Task<List<Appointment>> GetScheduleConflictsAsync(CancellationToken cancellationToken, long userId, TimeSpan startTime, TimeSpan endTime, DayOfWeek dayOfWeek, bool isBreak)
        {
            return isBreak ? await repository.GetScheduleConflictsAsync(cancellationToken, userId, startTime, endTime, dayOfWeek)
                           : await repository.GetScheduleConflictsByWeeklyPlanningAsync(cancellationToken, userId, startTime, endTime, dayOfWeek);
        }
    }
}
