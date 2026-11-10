<<<<<<< HEAD
using System.ComponentModel.DataAnnotations;
=======
﻿using System.ComponentModel.DataAnnotations;
>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72

namespace VIANDRIVEFULL.Models
{
    public class ClienteRegistroDto
    {
<<<<<<< HEAD
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
=======
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

>>>>>>> 8be03561b27d6ea1f119228c3c0891f11b09dc72
