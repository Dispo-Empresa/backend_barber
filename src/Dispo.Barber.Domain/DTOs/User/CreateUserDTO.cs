﻿using Dispo.Barber.Domain.Enums;

namespace Dispo.Barber.Domain.DTOs.User
{
    public class CreateUserDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string Phone { get; set; }
        public UserRole? Role { get; set; }
        public UserStatus? Status { get; set; }
        public bool? Active { get; set; } = true;
        public string? DeviceToken { get; set; }
        public List<long>? Services { get; set; }
    }
}
