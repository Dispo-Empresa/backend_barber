using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.DTOs.User;

namespace Dispo.Barber.Domain.DTOs.Chat
{
    public class InformationChatDTO
    {
        public string NameCompany { get; set; }
        public List<UserInformationDTO>? User { get; set; }
        public List<ServiceInformationDTO>? Services { get; set; }
        public List<ServiceInformationDTO>? ServicesUser { get; set; }
        public long BusinessUnities { get; set; }
    }
}
