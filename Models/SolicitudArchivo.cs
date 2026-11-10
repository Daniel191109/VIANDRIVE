public class SolicitudArchivo
{
    public int Id { get; set; }
    public int SolicitudId { get; set; }
    public string NombreArchivo { get; set; }
    public string RutaArchivo { get; set; }
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
}
