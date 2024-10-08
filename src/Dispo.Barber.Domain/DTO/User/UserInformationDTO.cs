namespace Dispo.Barber.Domain.DTO.User
{
    public class UserInformationDTO
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool Active { get; set; } = true;
    }
}
