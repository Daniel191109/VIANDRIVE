using System.ComponentModel.DataAnnotations;

namespace VIANDRIVEFULL.Models.DTOs
{
    public class CalificacionConductorDTO
    {
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5.")]
        public int Calificacion { get; set; }

        [StringLength(250, ErrorMessage = "El comentario no puede superar los 250 caracteres.")]
        public string? Comentario { get; set; }
    }

}
