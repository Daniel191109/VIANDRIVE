namespace VIANDRIVEFULL.Models
{
    public class Pago
    {
        public int Id { get; set; }
        public int ViajeId { get; set; }
        public int ClienteId { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string Metodo { get; set; } // "efectivo", "tarjeta", "simulado"
        public string Comprobante { get; set; }
    }

}
