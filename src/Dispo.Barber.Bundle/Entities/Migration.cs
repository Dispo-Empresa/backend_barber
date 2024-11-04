using System.ComponentModel.DataAnnotations.Schema;

namespace Dispo.Barber.Bundle.Entities
{
    public class Migration
    {
        [Column("DatabaseVersion")]
        public string Version { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}
