using Dispo.Barber.Domain.DTOs.Chat;
using Dispo.Barber.Domain.DTOs.Schedule;

namespace Dispo.Barber.Domain.Services.Interface
{
    public interface IInformationChatService
    {
        Task<InformationChatDTO> GetInformationChatByIdCompanyAsync(CancellationToken cancellationToken, long idCompany);
        Task<InformationChatDTO> GetInformationChatByIdUser(CancellationToken cancellationToken, long idUser);
        Task<InformationAppointmentChatDTO> GetInformationAppointmentChatByIdAppointment(CancellationToken cancellationToken, long idAppointment);
        Task<InformationChatUserDTO> GetInformationChatByIdService(List<long> idServices);
        Task<List<DayScheduleDto>> GetUserAppointmentsByUserIdAsync(CancellationToken cancellationToken, long idUser);
        Task<Dictionary<string, List<string>>> GetAvailableSlotsAsync(CancellationToken cancellationToken, AvailableSlotRequestDto availableSlotRequestDto);

    }
}
