namespace VIANDRIVEFULL.Models
{
    public class Cupon
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public decimal Descuento { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Usado { get; set; }
        public int? UsuarioId { get; set; }
    }

}
