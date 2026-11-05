using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class EstadoConductor
    {
        [Key]
        public int ConductorId { get; set; }

        public bool Disponible { get; set; }
        public DateTime UltimaActualizacion { get; set; }
    }

}

