﻿using Dispo.Barber.Domain.DTOs.Customer;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.Appointment
{
    public class CreateAppointmentDTO
    {
        public long Id { get; set; }
        public DateTime Date { get; set; }
        public string? CustomerObservation { get; set; }
        public string? AcceptedUserObservation { get; set; }
        public long? AcceptedUserId { get; set; }

        public required long BusinessUnityId { get; set; }
        public required List<long> Services { get; set; }
        public required AppointmentStatus? Status { get; set; }

        public CustomerDTO Customer { get; set; }
    }
}
