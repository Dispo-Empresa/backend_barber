﻿using Dispo.Barber.Domain.DTOs.Schedule;
using Dispo.Barber.Domain.DTOs.Service;
using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class UserDetailDTO
    {
        public long Id { get; set; }
        public byte[]? Photo { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public UserStatus Status { get; set; }
        public string Link { get; set; }
        public long CompanyId { get; set; }
        public UserRole Role { get; set; }
        public List<ScheduleDTO> Schedules { get; set; }
        public List<ServiceDetailDTO> Services { get; set; }
    }
}
