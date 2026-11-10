using System;
using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class CancelacionViaje
    {
        public int Id { get; set; }

        [Required]
        public int ViajeId { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        [RegularExpression(@"^(cliente|conductor)$", ErrorMessage = "Rol debe ser 'cliente' o 'conductor'")]
        public string Rol { get; set; }

        [Required]
        [MaxLength(250)]
        public string Motivo { get; set; }

        public DateTime FechaCancelacion { get; set; } = DateTime.Now;
    }
}
