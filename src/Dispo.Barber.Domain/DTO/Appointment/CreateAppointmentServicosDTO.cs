﻿using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.DTO.Appointment
{
    public class CreateAppointmentServicosDTO
    {
        public DateTime Date { get; set; }
        public string? CustomerObservation { get; set; }
        public string? AcceptedUserObservation { get; set; }
        public long? AcceptedUserId { get; set; }

        public required long BusinessUnityId { get; set; }
        public required List<long> ServiceIds { get; set; }
        public required AppointmentStatus Status { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
