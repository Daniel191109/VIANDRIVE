using System;

namespace VIANDRIVEFULL.Models.DTOs
{
    public class SolicitudServicioDto
    {
        public int ClienteId { get; set; }
        public int ConductorId { get; set; }
        public int VehiculoId { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }

        // COORDENADAS REQUERIDAS POR EL SERVICIO
        public double LatOrigen { get; set; }
        public double LngOrigen { get; set; }
        public double LatDestino { get; set; }
        public double LngDestino { get; set; }

        public double DistanciaKm { get; set; }
        public int TiempoEstimadoMin { get; set; }
        public bool HoraPico { get; set; }
        public decimal OfertaPasajero { get; set; }
        public string MedioPago { get; set; }
    }
}