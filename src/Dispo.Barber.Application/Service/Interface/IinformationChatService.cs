using Dispo.Barber.Domain.DTO.Chat;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IinformationChatService
    {
        Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(long idCompany);
        Task<InformationChatDTO> GetInformationChatByIdUser(long idUser);
        Task<InformationChatDTO> GetInformationChatByIdService(List<long> idServices);
    }
}
