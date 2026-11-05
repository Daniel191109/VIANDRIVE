namespace VIANDRIVEFULL.Models
{
    public class Mensaje
    {
        public int Id { get; set; }
        public int RemitenteId { get; set; }
        public int DestinatarioId { get; set; }
        public string Contenido { get; set; }
        public DateTime FechaEnvio { get; set; }
        public bool EsSoporte { get; set; }
    }

}
