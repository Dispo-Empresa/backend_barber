using Dispo.Barber.Domain.DTO.Appointment;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IAppointmentAppService
    {
        Task CreateAsync(CreateAppointmentDTO createAppointmentDTO);
        Task InformProblemAsync(long id, InformAppointmentProblemDTO informAppointmentProblemDTO);
    }
}
