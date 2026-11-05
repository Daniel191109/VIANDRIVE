namespace VIANDRIVEFULL.Models
{
    public class CalificacionConductor
    {
        public int Id { get; set; }
        public int SolicitudId { get; set; }
        public string UsuarioCalificadorId { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCalificacion { get; set; }
        public int UsuarioCalificadoId { get; set; }

        public SolicitudServicio Solicitud { get; set; } // si esta funciona bien
    }


}
