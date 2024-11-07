﻿using AutoMapper;
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
            appointment.Status = AppointmentStatus.Canceled;
            await repository.AddAsync(cancellationToken, appointment);
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
            repository.Update(appointment);
            await repository.SaveChangesAsync(cancellationToken);
        }

        //private async Task SendNotificationBySMS(Appointment appointment)
        //{
        //    smsService.SendMessageAsync(appointment.Customer.Phone, smsService.GenerateAppointmentMessage(appointment), MessageType.Sms);
        //}
    }
}