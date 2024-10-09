using Dispo.Barber.Domain.DTO.Appointment;
using Dispo.Barber.Domain.Entities;

namespace Dispo.Barber.Application.AppService.Interface
{
    public interface IAppointmentAppService
    {
        Task CreateAsync(CreateAppointmentDTO createAppointmentDTO);
        Task InformProblemAsync(long id, InformAppointmentProblemDTO informAppointmentProblemDTO);
        Task<Appointment> GetAsync(long id);
    }
}
