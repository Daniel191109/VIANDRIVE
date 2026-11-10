using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models
{
    public class ClienteRegistroDto
    {
        [Required, StringLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [Required, EmailAddress, StringLength(100)]
        public string Correo { get; set; } = string.Empty;

        [Required, RegularExpression(@"^\d{10}$", ErrorMessage = "Celular debe tener 10 dígitos")]
        public string Celular { get; set; } = string.Empty;

        // Se vuelve opcional para aceptar clientes que aún no seleccionan un medio de pago.
        [StringLength(50)]
        public string? MetodoPagoPreferido { get; set; } = "Efectivo";

        [Required, MaxLength(100)]
        public string Clave { get; set; } = string.Empty;
    }
}
