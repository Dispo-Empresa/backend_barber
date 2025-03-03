using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.Entities;
using System.Threading.Tasks;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IinformationChatService
    {
        Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken, long idCompany);
        Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser);
        Task<InformationAppointmentChatDTO> GetInformationAppointmentChatByIdAppointment(CancellationToken cancellationToken, long idAppointment);
        Task<InformationChatUserDTO> GetInformationChatByIdService(List<long> idServices);
        Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser);
        Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto);
        Task<bool> GetSuggestionAppointmentAsync();

    }
}
