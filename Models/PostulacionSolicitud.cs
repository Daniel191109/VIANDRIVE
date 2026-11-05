using System;

namespace VIANDRIVEFULL.Models
{
    public class PostulacionSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public int ConductorId { get; set; }
        public decimal TarifaOfrecida { get; set; }
        public string Estado { get; set; } = "Pendiente"; // "Pendiente", "Aceptada", "Rechazada"
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Propiedades de lectura rápida para evitar JOINs complejos
        public string NombreConductor { get; set; } = string.Empty;
        public string Vehiculo { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public double Calificacion { get; set; } = 5.0;
    }
}