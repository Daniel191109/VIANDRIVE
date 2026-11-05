namespace VIANDRIVEFULL.Models.DTOs
{
    public class SolicitudPendienteDto
    {
        public int Id { get; set; }
        public string Origen { get; set; }
        public string Destino { get; set; }
        public string Duracion { get; set; }
        public decimal Oferta { get; set; }

        // 🚨 AGREGA ESTAS 4 PROPIEDADES QUE FALTABAN:
        public double LatOrigen { get; set; }
        public double LngOrigen { get; set; }
        public double LatDestino { get; set; }
        public double LngDestino { get; set; }
    }
}