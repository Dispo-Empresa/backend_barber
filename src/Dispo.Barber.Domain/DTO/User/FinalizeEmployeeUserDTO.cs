namespace Dispo.Barber.Domain.DTO.User
{
    public class FinalizeEmployeeUserDTO
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public byte[]? Photo { get; set; }
        public string DeviceToken { get; set; }
    }
}