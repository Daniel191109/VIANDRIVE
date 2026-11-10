using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models.DTOs
{
    public class CalificacionClienteDTO
    {
        [Range(1, 5)]
        public int Calificacion { get; set; }

        [StringLength(250)]
        public string? Comentario { get; set; }
    }

}
