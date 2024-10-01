using Dispo.Barber.Application.AppService.Interface;
using Dispo.Barber.Domain.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dispo.Barber.API.Controllers
{
    [Route("api/v1/business-unities")]
    [ApiController]
    public class BusinessUnityController(IAppointmentAppService appointmentAppService) : ControllerBase
    {
    }
}
