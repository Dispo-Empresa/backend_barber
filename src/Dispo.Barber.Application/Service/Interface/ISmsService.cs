﻿using Dispo.Barber.Domain.Entities;
using Dispo.Barber.Domain.Utils;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface ISmsService
    {
        Task SendMessageAsync(string phoneNumberDestiny, string messageBody);
        string GenerateCreateAppointmentMessageSms(Appointment appointment);
        string GenerateCancelAppointmentMessageSms(Appointment appointment);
    }
}
