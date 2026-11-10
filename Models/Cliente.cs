using System;
using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; }

        [Required]
        [MaxLength(100)]
        public string Correo { get; set; }

        [Required]
        [MaxLength(20)]
        public string Celular { get; set; }

        [Required]
        [MaxLength(50)]
        public string MetodoPagoPreferido { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public string? FotoPerfilUrl { get; set; } // ← permite nulo
        public string FotoResumen => FotoPerfilUrl ?? "sin-foto";

        [Required]
        [MaxLength(100)]
        public string Clave { get; set; }




    }
}
