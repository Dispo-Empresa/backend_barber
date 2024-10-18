using Dispo.Barber.Domain.DTO.Chat;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IinformationChatService
    {
        Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken,long idCompany);
        Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser);
        Task<InformationChatDTO> GetInformationChatByIdService(List<long> idServices);
    }
}
