using AutoMapper;
using Dispo.Barber.Domain.DTOs.Appointment;
using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Exceptions;
using Dispo.Barber.Domain.Providers;
using Dispo.Barber.Domain.Repositories;
using Dispo.Barber.Domain.Services.Interfaces;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Utils.Extensions;

namespace Dispo.Barber.Domain.Services
{
    public class AppointmentService(IMapper mapper,
                                    IAppointmentRepository repository,
                                    ICustomerRepository customerRepository,
                                    INotificationSenderProvider notificationService,
                                    IUserRepository userRepository,
                                    ITwillioMessageSenderProvider twillioMessageSender,
                                    IServiceRepository serviceRepository) : IAppointmentService
    {
        private readonly string APPOINTMENT_CONFIRMATION_CONTENT_SID = Environment.GetEnvironmentVariable("APPOINTMENT_CONFIRMATION_CONTENT_SID") ?? "";
        private readonly string APPOINTMENT_CONFIRMATION_TEMPLATE = Environment.GetEnvironmentVariable("APPOINTMENT_CONFIRMATION_TEMPLATE") ?? "";

        private readonly string APPOINTMENT_CANCELLATION_CONTENT_SID = Environment.GetEnvironmentVariable("APPOINTMENT_CANCELLATION_CONTENT_SID") ?? "";
        private readonly string APPOINTMENT_CANCELLATION_TEMPLATE = Environment.GetEnvironmentVariable("APPOINTMENT_CANCELLATION_TEMPLATE") ?? "";

        private readonly string APPOINTMENT_RESCHEDULING_CONTENT_SID = Environment.GetEnvironmentVariable("APPOINTMENT_RESCHEDULING_CONTENT_SID") ?? "";
        private readonly string APPOINTMENT_RESCHEDULING_TEMPLATE = Environment.GetEnvironmentVariable("APPOINTMENT_RESCHEDULING_TEMPLATE") ?? "";

        public async Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO, bool notifyUsers = false, bool isChat = false)
        {
            // Dá pra melhorar todo esse método
            var appointment = mapper.Map<Appointment>(createAppointmentDTO);
            var user = await userRepository.GetFirstAsync(cancellationToken, createAppointmentDTO.AcceptedUserId ?? 0, "BusinessUnity.Company");

            if (user is not null && user.Status != UserStatus.Active)
            {
                throw new BusinessException("O usuário precisa estar ativo para aceitar um agendamento");
            }

            var existingCustomer = await customerRepository.GetFirstAsync(cancellationToken, w => w.Id == createAppointmentDTO.Customer.Id.Value && w.Appointments.Any(w => w.BusinessUnityId == createAppointmentDTO.BusinessUnityId));
            if (existingCustomer != null)
            {
                appointment.Customer = null;
                appointment.CustomerId = existingCustomer.Id;

                if (await customerRepository.HasMultipleAppointmentsAsync(cancellationToken, existingCustomer.Id))
                {
                    throw new BusinessException("Este usuário já possui muitos agendamentos confirmados.");
                }
            }
            else
            {
                appointment.Customer.Name = appointment.Customer.Name;
                appointment.Customer.Phone = StringUtils.FormatPhoneNumber(appointment.Customer.Phone);

                var existingIdByPhone = await customerRepository.GetCustomerIdByPhoneAsync(appointment.Customer.Phone, cancellationToken);
                if (existingIdByPhone.IsValid() && !isChat)
                    throw new AlreadyExistsException("Este número já está cadastrado para um cliente da barbearia.");
            }

            appointment.Status = AppointmentStatus.Scheduled;
            await repository.AddAsync(cancellationToken, appointment);
            await repository.SaveChangesAsync(cancellationToken);

            appointment.AcceptedUser = user;
            appointment.Customer = await customerRepository.GetAsync(cancellationToken, appointment.CustomerId);

            var selectedServices = await serviceRepository.GetListServiceAsync(createAppointmentDTO.Services.ToList());
            //await SendWhatsAppMessageAppointmentConfirmationAsync(cancellationToken, appointment, selectedServices, false);

            if (notifyUsers)
                await SendNotificationToApp(cancellationToken, appointment, "Agendamento Confirmado", notificationService.GenerateCreateAppointmentMessageApp(appointment), NotificationType.NewAppointment);
        }

        public async Task Reschedule(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO)
        {
            // Dá pra melhorar todo esse método

            var oldAppointment = await repository.GetAppointmentByIdAsync(cancellationToken, createAppointmentDTO.Id) ?? throw new NotFoundException("Agendamento não existe.");
            oldAppointment.Status = AppointmentStatus.Canceled;

            var newAppointment = mapper.Map<Appointment>(createAppointmentDTO);
            newAppointment.Id = 0;
            var user = await userRepository.GetFirstAsync(cancellationToken, createAppointmentDTO.AcceptedUserId ?? 0, "BusinessUnity.Company");

            if (user is not null && user.Status != UserStatus.Active)
                throw new BusinessException("O usuário precisa estar ativo para aceitar um agendamento");

            newAppointment.Customer = null;
            newAppointment.CustomerId = createAppointmentDTO.Customer.Id.Value;
            newAppointment.Status = AppointmentStatus.Scheduled;

            repository.Update(oldAppointment);
            await repository.AddAsync(cancellationToken, newAppointment);
            await repository.SaveChangesAsync(cancellationToken);

            newAppointment.AcceptedUser = user;
            newAppointment.Customer = await customerRepository.GetAsync(cancellationToken, newAppointment.CustomerId);

            var selectedServices = await serviceRepository.GetListServiceAsync(createAppointmentDTO.Services.ToList());
            await SendWhatsAppMessageAppointmentConfirmationAsync(cancellationToken, newAppointment, selectedServices, true);

            await SendNotificationToApp(cancellationToken, newAppointment, "Reagendamento confirmado", notificationService.GenerateCreateAppointmentMessageApp(newAppointment), NotificationType.RescheduleAppointment);
        }

        public async Task CancelAppointmentAsync(CancellationToken cancellationToken, long id, bool notifyUsers = false)
        {
            var appointment = await repository.GetAppointmentByIdAsync(cancellationToken, id) ?? throw new NotFoundException("Agendamento não existe.");

            if (appointment.Status == AppointmentStatus.Canceled)
                throw new BusinessException("O agendamento já está cancelado.");

            appointment.Status = AppointmentStatus.Canceled;

            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);

            if (notifyUsers)
                await SendNotificationToApp(cancellationToken, appointment, "Agendamento Cancelado", notificationService.GenerateCancelAppointmentMessageApp(appointment), NotificationType.CanceledAppointment);
            else
                await SendWhatsAppMessageAppointmentCancellationAsync(cancellationToken, appointment);
        }

        public async Task CancelAppointmentsAsync(CancellationToken cancellationToken, List<long> appointmentIds)
        {
            foreach (var appointmentId in appointmentIds)
            {
                var appointment = await repository.GetAppointmentByIdAsync(cancellationToken, appointmentId) ?? throw new NotFoundException("Agendamento não existe.");
                appointment.Status = AppointmentStatus.Canceled;

                repository.Update(appointment);
                await repository.SaveChangesAsync(cancellationToken);

                await SendWhatsAppMessageAppointmentCancellationAsync(cancellationToken, appointment);
            }
        }

        public async Task CancelAllByDateAsync(CancellationToken cancellationToken, long userId, DateTime date)
        {
            await repository.CancelAllByDateAsync(cancellationToken, userId, date);
            var appointmentList = await repository.GetAppointmentByUserAndDateIdSync(cancellationToken, userId, date);
            var notifiedCustomers = new HashSet<long>();

            foreach (var appointment in appointmentList)
            {
                if (!notifiedCustomers.Contains(appointment.CustomerId))
                {
                    await SendWhatsAppMessageAppointmentCancellationAsync(cancellationToken, appointment);
                    notifiedCustomers.Add(appointment.CustomerId);
                }
            }
            await repository.SaveChangesAsync(cancellationToken);
        }

        public async Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO)
        {
            var appointment = await repository.GetAppointmentByIdAsync(cancellationToken, id) ?? throw new NotFoundException("Agendamento não existe.");

            appointment.AcceptedUserObservation = informAppointmentProblemDTO.Problem;
            appointment.Status = AppointmentStatus.Canceled;

            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);

            await SendWhatsAppMessageAppointmentCancellationAsync(cancellationToken, appointment);
        }

        public async Task<Appointment> GetAsync(CancellationToken cancellationToken, long id)
        {
            return await repository.GetAsync(cancellationToken, id) ?? throw new NotFoundException("Agendamento não existe.");
        }

        public async Task<List<Appointment>> GetNextAppointmentsAsync(CancellationToken cancellationToken, long userId)
        {
            return await repository.GetNextAppointmentsAsync(cancellationToken, userId);
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

        private async Task SendNotificationToApp(CancellationToken cancellationToken, Appointment appointment, string tittle, string body, NotificationType notificationType)
        {
            await notificationService.NotifyAsync(cancellationToken, appointment.AcceptedUser.DeviceToken, tittle, body, notificationType);

            var acceptedUser = await userRepository.GetAsync(cancellationToken, appointment.AcceptedUserId.Value);
            acceptedUser.UnreadNotificationsCount++;

            await repository.SaveChangesAsync(cancellationToken);
        }

        private async Task SendWhatsAppMessageAppointmentConfirmationAsync(CancellationToken cancellationToken, Appointment appointment, List<Service> selectedServices, bool rescheduling)
        {
            var appointmentConfirmationMessage = new AppointmentWhatsAppMessageDTO();
            appointmentConfirmationMessage.BarbershopName = appointment.AcceptedUser?.BusinessUnity?.Company?.Name;
            appointmentConfirmationMessage.Date = appointment.Date.ToString("dd/MM/yyyy");
            appointmentConfirmationMessage.Time = appointment.Date.ToString("HH:mm");
            appointmentConfirmationMessage.ProfessionalName = appointment.AcceptedUser?.Name;
            appointmentConfirmationMessage.ServicesNames = string.Join(", ", selectedServices.Select(w => w.Description));
            appointmentConfirmationMessage.Link = appointment.CancellationEntireSlug();

            if (rescheduling)
                await twillioMessageSender.SendWhatsAppMessageAsync(appointment.Customer.Phone, APPOINTMENT_RESCHEDULING_TEMPLATE, APPOINTMENT_RESCHEDULING_CONTENT_SID, appointmentConfirmationMessage.ToConfirmation());
            else
                await twillioMessageSender.SendWhatsAppMessageAsync(appointment.Customer.Phone, APPOINTMENT_CONFIRMATION_TEMPLATE, APPOINTMENT_CONFIRMATION_CONTENT_SID, appointmentConfirmationMessage.ToConfirmation());
        }

        private async Task SendWhatsAppMessageAppointmentCancellationAsync(CancellationToken cancellationToken, Appointment appointment)
        {
            var appointmentConfirmationMessage = new AppointmentWhatsAppMessageDTO();
            appointmentConfirmationMessage.ProfessionalName = appointment.AcceptedUser?.Name;
            appointmentConfirmationMessage.BarbershopName = appointment.AcceptedUser?.BusinessUnity?.Company?.Name;
            appointmentConfirmationMessage.Date = appointment.Date.ToString("dd/MM/yyyy");
            appointmentConfirmationMessage.Time = appointment.Date.ToString("HH:mm");
            appointmentConfirmationMessage.Link = appointment.AcceptedUser?.BusinessUnity?.EntireSlug();

            await twillioMessageSender.SendWhatsAppMessageAsync(appointment.Customer.Phone, APPOINTMENT_CANCELLATION_TEMPLATE, APPOINTMENT_CANCELLATION_CONTENT_SID, appointmentConfirmationMessage.ToCancellation());
        }
    }
}
