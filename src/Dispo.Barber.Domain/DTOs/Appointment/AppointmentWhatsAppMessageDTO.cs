namespace Dispo.Barber.Domain.DTOs.Appointment
{
    public class AppointmentWhatsAppMessageDTO
    {
        public string BarbershopName { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string ProfessionalName { get; set; }
        public string ServicesNames { get; set; }
        public string Link { get; set; }

        public string[] ToConfirmation()
        {
            return [
                     BarbershopName,
                     Date,
                     Time,
                     ProfessionalName,
                     ServicesNames,
                     Link
                   ];
        }

        public string[] ToCancellation()
        {
            return [
                     ProfessionalName,
                     BarbershopName,
                     Date,
                     Time,
                     Link
                   ];
        }
    }
}
