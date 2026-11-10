namespace VIANDRIVEFULL.Models
{
    public class TokenRequest
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string Tipo { get; set; } // 👈 Vital para que reciba "login", "registro_conductor" o "registro_cliente"
    }
}