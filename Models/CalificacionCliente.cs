namespace VIANDRIVEFULL.Models
{
    public class CalificacionCliente
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string UsuarioAltaId { get; set; }
        public DateTime FechaAlta { get; set; }
        public DateTime FechaCalificacion { get; set; }
        public string ClienteId { get; set; }
        public int Calificacion { get; set; }
        public string? Comentario { get; set; }

        public SolicitudServicio Solicitud { get; set; }
    }

}
