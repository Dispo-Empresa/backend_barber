namespace Dispo.Barber.Domain.DTOs.User
{
    public class UpdateUserDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string Phone { get; set; }
        public string? Password { get; set; }
        public string? DeviceToken { get; set; }
    }
}
