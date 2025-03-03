namespace Dispo.Barber.Domain.DTOs.Customer
{
    public class CustomerDetailDTO
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public DateTime? LastAppointment { get; set; }
        public int Frequency { get; set; }
    }
}
