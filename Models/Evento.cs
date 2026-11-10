using System.ComponentModel.DataAnnotations;
using VIANDRIVEFULL.Models;

public class Evento
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Tipo { get; set; }

    [Required]
    [MaxLength(300)]
    public string Descripcion { get; set; }

    public DateTime Fecha { get; set; } = DateTime.Now;

    public int? ConductorId { get; set; }
    public Conductor Conductor { get; set; }
}
