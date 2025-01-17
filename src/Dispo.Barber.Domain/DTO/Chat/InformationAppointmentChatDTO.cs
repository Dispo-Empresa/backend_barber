
using Dispo.Barber.Domain.DTO.BusinessUnity;
using Dispo.Barber.Domain.DTO.Customer;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Domain.DTO.Chat
{
    public class InformationAppointmentChatDTO
    {
        public string NameCompany { get; set; }
        public long? IdUser { get; set; }
        public string NameUser { get; set; }
        public string NameCustomer { get; set; }
        public DateTime DateAppointment { get; set; }
    }
}
