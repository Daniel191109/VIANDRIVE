namespace VIANDRIVEFULL.Models.DTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool ConToken { get; set; } = false; // opcional
    }
}
