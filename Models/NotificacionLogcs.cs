using System;

namespace VIANDRIVEFULL.Models
{
    public class NotificacionLog
    {
        public int Id { get; set; }
        public string Tipo { get; set; } // "Correo" o "SMS"
        public string Destinatario { get; set; }
        public string Mensaje { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string Estado { get; set; } // "Enviado" o "Error"

        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }

    }
}

