namespace VIANDRIVEFULL.Models
{
    public class Tarifa
    {
        public int Id { get; set; }
        public decimal Base { get; set; }
        public decimal PorKm { get; set; }
        public decimal PorHoraPico { get; set; }
        public TimeSpan HoraInicioPico { get; set; }
        public TimeSpan HoraFinPico { get; set; }
    }

}
