﻿using Dispo.Barber.Domain.Enums;
using Dispo.Barber.Domain.Utils;
using Dispo.Barber.Domain.Utils.Constants;

namespace Dispo.Barber.Domain.Entities
{
    public class Appointment : EntityBase
    {
        public DateTime Date { get; set; }
        public DateTime AccomplishedDate { get; set; }
        public string? CustomerObservation { get; set; }
        public string? AcceptedUserObservation { get; set; }

        public required long BusinessUnityId { get; set; }
        public required BusinessUnity BusinessUnity { get; set; }

        public required long CustomerId { get; set; }
        public required Customer Customer { get; set; }

        public long? AcceptedUserId { get; set; }
        public User? AcceptedUser { get; set; }
        public required List<ServiceAppointment> Services { get; set; }

        public AppointmentStatus Status { get; set; }

        public string CancellationEntireSlug()
        {
            var appointmentIdEncripted = CryptoHelper.Encrypt(Id.ToString());
            return $"{Links.AuraChatCancellationLink}{appointmentIdEncripted}";
        }
    }
}
