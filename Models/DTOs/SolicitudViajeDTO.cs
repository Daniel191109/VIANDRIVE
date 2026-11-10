using System;

namespace VIANDRIVEFULL.Models
{
    public class SolicitudViajeDto
    {
        public int UsuarioId { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string Destino { get; set; } = string.Empty;
        public DateTime FechaHora { get; set; }
        public decimal Tarifa { get; set; }
        public string Estado { get; set; } = "Pendiente";

        public string Duracion { get; set; } = string.Empty;
        public double LatOrigen { get; set; }
        public double LngOrigen { get; set; }
        public double LatDestino { get; set; }
        public double LngDestino { get; set; }
    }
}