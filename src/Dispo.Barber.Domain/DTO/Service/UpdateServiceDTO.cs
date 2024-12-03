
using System.ComponentModel.DataAnnotations;

namespace Dispo.Barber.Domain.DTO.Service
{
    public class UpdateServiceDTO
    {
        [Required]
        public required string Description { get; set; }

        [Required]
        [Range(1, double.MaxValue, ErrorMessage = "Preço inválido")]
        public required double Price { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Duração inválida.")]
        public required int Duration { get; set; }
    }
}
