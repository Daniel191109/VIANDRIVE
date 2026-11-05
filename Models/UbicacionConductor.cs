using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class UbicacionConductor
    {
        [Key]
        public int ConductorId { get; set; }

        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
