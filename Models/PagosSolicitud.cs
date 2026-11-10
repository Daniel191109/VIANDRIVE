namespace VIANDRIVEFULL.Models
{
    public class PagosSolicitud
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string MedioPago { get; set; } // Efectivo, Tarjeta, QR
        public EstadoPago EstadoPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
        public string ReferenciaTransaccion { get; set; }

        public SolicitudServicio Solicitud { get; set; }
    }

    public enum EstadoPago
    {
        Pendiente = 0,
        Aprobado = 1,
        Rechazado = 2
    }

}


