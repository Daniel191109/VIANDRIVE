using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VIANDRIVEFULL.Models
{
    public class Vehiculo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(10)]
        public string Placa { get; set; }

        [Required]
        [MaxLength(30)]
        public string Marca { get; set; }

        [Required]
        [MaxLength(30)]
        public string Modelo { get; set; }

        [Required]
        [MaxLength(20)]
        public string Color { get; set; }

        [Range(1900, 2100)]
        public int Año { get; set; }

        [ForeignKey("Conductor")]
        public int ConductorId { get; set; }

        public Conductor Conductor { get; set; }
    }
}
