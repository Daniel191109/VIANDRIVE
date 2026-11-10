namespace VIANDRIVEFULL.Models
{
    public class Viaje
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int? ConductorId { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Estado { get; set; } // "pendiente", "aceptado", "finalizado", "cancelado"
        public DateTime FechaSolicitud { get; set; }
        public string RutaGoogleMaps { get; set; }
    }

}
