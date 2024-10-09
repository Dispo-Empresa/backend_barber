namespace Dispo.Barber.Domain.DTO.User
{
    public class UserInformationDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public bool Active { get; set; } = true;
    }
}
