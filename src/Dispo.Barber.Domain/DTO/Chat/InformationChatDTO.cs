
using Dispo.Barber.Domain.DTO.BusinessUnity;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.DTO.User;

namespace Dispo.Barber.Domain.DTO.Chat
{
    public class InformationChatDTO
    {
        public string NameCompany { get; set; }
        public List<UserInformationDTO>? User { get; set; }
        public List<ServiceInformationDTO>? Services { get; set; }
        public long BusinessUnities { get; set; }
    }
}
