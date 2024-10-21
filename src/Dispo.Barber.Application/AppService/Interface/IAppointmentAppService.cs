using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IAppointmentAppService
    {
        Task CreateAsync(CancellationToken cancellationToken, CreateAppointmentDTO createAppointmentDTO);

        Task InformProblemAsync(CancellationToken cancellationToken, long id, InformAppointmentProblemDTO informAppointmentProblemDTO);

        Task<Appointment> GetAsync(CancellationToken cancellationToken, long id);

        Task CancelAppointmentAsync(long id);
    }
}
