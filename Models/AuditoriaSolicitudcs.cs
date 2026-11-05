namespace VIANDRIVEFULL.Models
{
    public class AuditoriaSolicitud
    {
        public int Id { get; set; } // Clave primaria
        public int SolicitudId { get; set; } // FK a SolicitudesServicio
        public string Accion { get; set; } // Ej: "Pago registrado", "Calificación aplicada"
        public string Usuario { get; set; } // Nombre del usuario que ejecutó la acción
        public DateTime Fecha { get; set; } // Fecha de la acción

        public SolicitudServicio Solicitud { get; set; } // Navegación opcional
    }
}
