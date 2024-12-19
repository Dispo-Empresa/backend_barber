using Dispo.Barber.Domain.Enum;

namespace Dispo.Barber.Domain.DTO.Service
{
    public class ServiceListDTO
    {
        public long Id { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public int Duration { get; set; }
        public ServiceStatus Status { get; set; }
    }
}
