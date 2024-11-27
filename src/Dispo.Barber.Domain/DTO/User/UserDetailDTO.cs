using Dispo.Barber.Domain.DTO.Schedule;
using Dispo.Barber.Domain.DTO.Service;
using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.User
{
    public class UserDetailDTO
    {
        public long Id { get; set; }
        public string Picture { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string Link { get; set; }
        public List<ScheduleDTO> Schedules { get; set; }
        public List<ServiceDetailDTO> Services { get; set; }
    }
}
