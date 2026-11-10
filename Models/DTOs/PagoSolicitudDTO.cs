namespace VIANDRIVEFULL.Models.DTOs
{
    public class PagoSolicitudDTO
    {
        public string MedioPago { get; set; }
        public EstadoPago EstadoPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string ReferenciaTransaccion { get; set; }
    }

}
