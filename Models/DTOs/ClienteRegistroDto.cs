using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class ClienteRegistroDto
    {
        [Required]
        public string NombreCompleto { get; set; }

        [Required]
        [EmailAddress]
        public string Correo { get; set; }

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Celular debe tener 10 dígitos")]
        public string Celular { get; set; }

        [Required]
        public string MetodoPagoPreferido { get; set; }

        [Required]
        [MaxLength(100)]
        public string Clave { get; set; }

        //[Required]
        //[RegularExpression(@"^(Correo|SMS)$", ErrorMessage = "Debe seleccionar Correo o SMS")]
        ////public string EnviarConfirmacionPor { get; set; } // ← Este es el nuevo campo
    }
}

