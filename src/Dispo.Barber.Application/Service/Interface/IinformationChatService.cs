using Dispo.Barber.Domain.DTO.Chat;
using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.Service.Interface
{
    public interface IinformationChatService
    {
        Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken,long idCompany);
        Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser);
        Task<InformationChatDTO> GetInformationChatByIdService(List<long> idServices);
        Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser);
    }
}
