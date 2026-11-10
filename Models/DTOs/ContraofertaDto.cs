using System.Text.Json.Serialization;

namespace VIANDRIVEFULL.Models.DTOs
{
    public class ContraofertaDto
    {
        [JsonPropertyName("solicitudId")]
        public int SolicitudId { get; set; }

        [JsonPropertyName("conductorId")]
        public int ConductorId { get; set; }

        [JsonPropertyName("tarifaOfrecida")]
        public decimal TarifaOfrecida { get; set; }

        [JsonPropertyName("esContraoferta")]
        public bool EsContraoferta { get; set; }

        [JsonPropertyName("latConductor")]
        public double LatConductor { get; set; }

        [JsonPropertyName("lngConductor")]
        public double LngConductor { get; set; }
    }
}