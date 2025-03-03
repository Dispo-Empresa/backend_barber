namespace Dispo.Barber.Domain.DTOs.Chat
{
    public class InformationAppointmentChatDTO
    {
        public string NameCompany { get; set; }
        public long? IdUser { get; set; }
        public string NameUser { get; set; }
        public string NameCustomer { get; set; }
        public string Phone { get; set; }
        public DateTime DateAppointment { get; set; }
    }
}
